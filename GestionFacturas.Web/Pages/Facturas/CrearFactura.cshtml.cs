using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionFacturas.Dominio;
using GestionFacturas.AccesoDatosSql;
using Microsoft.EntityFrameworkCore;
using EditorFactura = GestionFacturas.Web.Pages.Facturas.EditorTemplates.EditorFactura;

namespace GestionFacturas.Web.Pages.Facturas
{
    public class CrearFacturaModel : PageModel
    {
        public static readonly string NombrePagina = "/Facturas/CrearFactura";

        private readonly SqlDb _db;

        public string HrefCancelar { get; set; } = string.Empty;

        public CrearFacturaModel( SqlDb db)
        {
            _db = db;
            
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id.HasValue)
            {
                var factura = await _db.Facturas
                    .Include(m => m.Lineas)
                    .FirstAsync(m => m.Id == id.Value);
                
                var ultimaFacturaSerie = await _db.ObtenerUlitmaFacturaDeLaSerie(factura.SerieFactura);
                var cliente = await _db.Clientes.FindAsync(factura.IdComprador);
                
                Editor = EditorFactura.GenerarNuevoEditorFacturaDuplicado(factura, ultimaFacturaSerie, cliente!);
                
                HrefCancelar = Url.Page(DetallesFacturaModel.NombrePagina, new { id = id.Value })!;
            }
            else
            {
                var ultimaFacturaSerie = await _db.ObtenerUlitmaFacturaDeLaSerie(string.Empty);
                var cliente = await _db.Clientes.FindAsync(IdCliente);

                Editor = EditorFactura.ObtenerEditorFacturaParaCrearNuevaFactura(ultimaFacturaSerie, cliente);
                HrefCancelar = Url.Page(ListaGestionFacturasModel.NombrePagina)!;
            }

            Editor.IdUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return Page();
        }
        [BindProperty(SupportsGet = true)]
        public int? IdCliente { get; set; }

        [BindProperty]
        public EditorFactura Editor { get; set; }  = new ();

        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var factura =  await CrearFacturaAsync(Editor);
            
            return RedirectToPage(DetallesFacturaModel.NombrePagina, new { factura.Id });
        }

        public async Task<Factura> CrearFacturaAsync(EditorFactura editor)
        {
            var factura = new Factura();

            var comprador = await _db
                .Clientes
                .FirstAsync(m => m.Id == editor.IdComprador);

            factura.Comprador = comprador;

            EditorFactura.ModificarFactura(editor, factura, _db);

            _db.Facturas.Add(factura);

            await _db.SaveChangesAsync();

            return factura;
        }

    }
    
}
