using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionFacturas.Aplicacion;
using System.Net;
using CSharpFunctionalExtensions;
using GestionFacturas.AccesoDatosSql;
using GestionFacturas.Dominio;
using GestionFacturas.Dominio.Infra;
using GestionFacturas.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionFacturas.Web.Pages.Facturas
{
    
    public class EnviarFacturaPorEmailModel : PageModel
    {
        public const string NombrePagina = @"/Facturas/EnviarFacturaPorEmail";
        
        private readonly IWebHostEnvironment _env;
        private readonly SqlDb _db;
        private readonly MailSettings _mailSettings;
        private readonly IServicioEmail _email;
        private readonly IVerifactuServicio _verifactu;


        public EnviarFacturaPorEmailModel(
            IWebHostEnvironment env, 
            MailSettings mailSettings, 
            SqlDb db, 
            IServicioEmail email, 
            IVerifactuServicio verifactu)
        {
            _env = env;
            _mailSettings = mailSettings;
            _db = db;
            _email = email;
            _verifactu = verifactu;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public string NumeroFactura { get; set; } = string.Empty;

        [BindProperty]
        public bool MostrarCheckEnviarAHacienda { get; set; }

        [BindProperty]
        public bool EnviarAHacienda { get; set; }

        [BindProperty] public EditorEmail EditorEmail { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {

            var factura = await _db.Facturas
                .Include(m => m.Lineas)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (factura == null)
            {
                return NotFound();
            }
            

            Id = factura.Id;

            NumeroFactura = factura.NumeroFactura;

            MostrarCheckEnviarAHacienda = !factura.EnviadaAHacienda;
            EnviarAHacienda = MostrarCheckEnviarAHacienda;

            EditorEmail = new EditorEmail
            {
                Remitente = factura.VendedorEmail ?? string.Empty,
                Asunto = $"Factura {factura.NumeroFactura} Bahía Code",
                ContenidoTexto = @"Hola,",
                Destinatarios = factura.CompradorEmail ?? string.Empty,
                DisplayName = factura.VendedorNombreOEmpresa
            };

            CargarCombos();

            return Page();
        }

        private void CargarCombos()
        {
            EditorEmail.EmailRemitentes = _mailSettings.ReplyToList().Select(m => new SelectListItem(m, m)).ToList();
            EditorEmail.NombresRemitentes = _mailSettings.DisplayNamesList().Select(m => new SelectListItem(m, m)).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                CargarCombos();
                return Page();
            }

            var factura = await _db.Facturas
                .Include(m => m.Lineas)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (factura is null)
                return NotFound();


            if (EnviarAHacienda)
            {
                var envio = await _verifactu.EnviarAltaFacturaAsync(factura.Id);
                if (envio.IsFailure)
                {
                    ModelState.AddModelError(string.Empty, envio.Error);
                    CargarCombos();
                    return Page();
                }
            }


            var qr = _verifactu.ObtenerQr(factura);

            if (qr.IsFailure)
                return BadRequest(qr.Error);

            var mensaje = GenerarMensajeEmail(EditorEmail, factura, qr.Value);

            if (mensaje.IsFailure)
            {
                ModelState.AddModelError("", mensaje.Error);
                CargarCombos();
                return Page();
            }

            await _email.EnviarMensajeAsync(mensaje.Value);

            factura.EstadoFactura = EstadoFacturaEnum.Enviada;
            await _db.SaveChangesAsync();
            

            var numeroFacturaCodificada = WebUtility.UrlEncode(factura.NumeroFactura);

            return RedirectToPage(EnviarFacturaPorEmailModelConfirmadoModel.NombrePagina,new { numeroFacturaEnviada = numeroFacturaCodificada } );
        }

        private Result<MensajeEmail> GenerarMensajeEmail(EditorEmail editorEmail, Factura factura, MemoryStream qr)
        {
            var informeLocal = GeneraLocalReportFactura.GenerarInformeLocalFactura(factura, qr, _env.WebRootPath);

            byte[] pdf = informeLocal.Render("PDF");

            var destinatarios = editorEmail.Destinatarios
                .Split(new[] { ';' },
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries)
                .Select(m => DireccionEmail.Crear(m, m)).ToList();


            var adjuntos = new List<ArchivoAdjunto>
            {
                new()
                {
                    Archivo = pdf,
                    MimeType = "application/pdf",
                    Nombre = factura.NumeroYEmpresaFactura() + ".pdf"
                }
            };

            if (editorEmail.ArchivosAdjuntos != null)
                foreach (var archivo in editorEmail.ArchivosAdjuntos)
                {
                    using var memoryStream = new MemoryStream();
                    archivo.CopyTo(memoryStream);

                    // Upload the file if less than 2 MB
                    if (memoryStream.Length < 2097152)
                    {
                        adjuntos.Add(new ArchivoAdjunto
                        {
                            Archivo = memoryStream.ToArray(),
                            MimeType = archivo.ContentType,
                            Nombre = archivo.FileName
                        });
                    }
                    else
                    {
                        return Result.Failure<MensajeEmail>("Los archivos adjuntos tienen un máximo de 2Mb");
                    }
                }


            var mensaje = MensajeEmail.Crear(
                editorEmail.Asunto,
                editorEmail.ContenidoTexto,
                destinatarios,
                DireccionEmail.Crear(
                    editorEmail.DisplayName,
                    Email.FromString(editorEmail.Remitente).Value),
                adjuntos
            ).Value;


            return mensaje;
        }
    }

    
}
