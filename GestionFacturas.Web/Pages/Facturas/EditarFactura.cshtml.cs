using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionFacturas.Aplicacion;
using GestionFacturas.Dominio;
using EditorFactura = GestionFacturas.Web.Pages.Facturas.EditorTemplates.EditorFactura;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using GestionFacturas.AccesoDatosSql;

namespace GestionFacturas.Web.Pages.Facturas
{
    
    public class EditarFacturaModel : PageModel
    {
        public const string NombrePagina = @"/Facturas/EditarFactura";

        private readonly SqlDb _db;

        public EditarFacturaModel(SqlDb db)
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


            this.Editor = new EditorFactura(factura);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var factura = await _db.Facturas
                .Include(m => m.Lineas)
                .FirstAsync(m => m.Id == Editor.Id);

            var validacion = factura.ValidarEsPosibleModificar();

            if (validacion.IsFailure)
            {
                ModelState.AddModelError(string.Empty, validacion.Error);
                return Page();
            }


            EditorFactura.ModificarFactura(Editor, factura, _db);
            
            _db.Entry(factura).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToPage(DetallesFacturaModel.NombrePagina, new { Editor.Id });
        }

    }
}
