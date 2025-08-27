using GestionFacturas.AccesoDatosSql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionFacturas.Web.Pages.Facturas.DisplayTemplates;
using Microsoft.EntityFrameworkCore;

namespace GestionFacturas.Web.Pages.Facturas
{
    public class DetallesFacturaModel : PageModel
    {
        public const string NombrePagina = @"/Facturas/DetallesFactura";

        private readonly SqlDb _db;

        public DetallesFacturaModel(SqlDb servicioFactura)
        {
            _db = servicioFactura;
        }

      public VisorFactura Factura { get; set; } = default!;
   

      public bool MostrarBotonEnviarAHacienda { get; set; }

      public async Task<IActionResult> OnGetAsync(int id)
        {
            var factura = 
                await _db.Facturas
                    .AsNoTracking()
                    .Include(m => m.Lineas)
                        .FirstAsync(m => m.Id == id);
            
            Factura = new VisorFactura(factura);

            MostrarBotonEnviarAHacienda = !factura.EnviadaAHacienda;

            
            return Page();

        }
    }
}
