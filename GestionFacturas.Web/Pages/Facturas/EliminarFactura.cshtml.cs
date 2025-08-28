using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EditorFactura = GestionFacturas.Web.Pages.Facturas.EditorTemplates.EditorFactura;
using GestionFacturas.AccesoDatosSql;
using Microsoft.EntityFrameworkCore;

namespace GestionFacturas.Web.Pages.Facturas
{
    
    public class EliminarFacturaModel : PageModel
    {
        public const string NombrePagina = @"/Facturas/EliminarFactura";

        private readonly SqlDb _db;

        public EliminarFacturaModel(SqlDb db)
        {
            _db = db;
        }

        [BindProperty]
        public EditorFactura Editor { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var factura = await _db.Facturas
                .Include(m => m.Lineas)
                .FirstAsync(m => m.Id == id); 

            Editor = new EditorFactura(factura);
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var factura = await _db.Facturas
                    .Include(m => m.Lineas)
                    .FirstAsync(m => m.Id == Editor.Id);


            var validacion = factura.ValidarEsPosibleEliminar();

            if (validacion.IsFailure)
            {
                ModelState.AddModelError(string.Empty, validacion.Error);
                return Page();
            }

            while (factura.Lineas.Any())
            {
                var linea = factura.Lineas.First();
                _db.FacturasLineas.Remove(linea);
            }

            _db.Facturas.Remove(factura);

            await _db.SaveChangesAsync();

            return RedirectToPage("/Facturas/EliminarFacturaConfirmado");
        }


    }
}
