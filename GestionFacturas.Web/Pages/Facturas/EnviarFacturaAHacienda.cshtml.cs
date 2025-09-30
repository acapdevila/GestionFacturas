using GestionFacturas.AccesoDatosSql;
using GestionFacturas.Aplicacion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GestionFacturas.Web.Pages.Facturas
{
    
    public class EnviarFacturaAHaciendaModel : PageModel
    {
        public const string NombrePagina = @"/Facturas/EnviarFacturaAHacienda";
        
        private readonly SqlDb _db;
        private readonly IVerifactuServicio _verifactu;


        public EnviarFacturaAHaciendaModel(SqlDb db, IVerifactuServicio verifactuServicio)
        {
            _db = db;
            _verifactu = verifactuServicio;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public string NumeroFactura { get; set; } = string.Empty;

        [BindProperty]
        public string EstadoFactura { get; set; } = string.Empty;

        [BindProperty]
        public string Cliente { get; set; } = string.Empty;

        [BindProperty]
        public string Total { get; set; } = string.Empty;


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
            EstadoFactura = factura.EstadoFactura.ToString();
            Cliente = factura.CompradorNombreOEmpresa;
            Total = factura.ImporteTotal().ToString("C2");
            

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            
            var resultado = await _verifactu.EnviarAltaFacturaAsync(Id);

            if (resultado.IsFailure)
            {
                ModelState.AddModelError(string.Empty, resultado.Error);
                return Page();
            }


            return RedirectToPage(DetallesFacturaModel.NombrePagina, new { Id });

        }

    
    }

    
}
