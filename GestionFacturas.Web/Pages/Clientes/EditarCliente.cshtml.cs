using GestionFacturas.AccesoDatosSql;
using GestionFacturas.Web.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Omu.ValueInjecter;

namespace GestionFacturas.Web.Pages.Clientes;

[BindProperties]
public class EditarClienteModel : PageModel
{
    
    public static readonly string NombrePagina = "/" + nameof(Clientes) + "/" + nameof(EditarClienteModel).QuitarStringModel();

    private readonly SqlDb _db;
    
    public EditarClienteModel(SqlDb db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public long Id { get; set; }

    public EditorClienteVm Editor { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {
        var cliente = await _db.Clientes.FirstOrDefaultAsync(m => m.Id == Id);
        if (cliente is null)
        {
            return NotFound();
        }

        Editor.InjectFrom(cliente);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var cliente = await _db.Clientes.FirstAsync(m => m.Id == Id);
        cliente.InjectFrom(Editor);

        await  _db.SaveChangesAsync();

        return RedirectToPage(ListaGestionClientesModel.NombrePagina);
    }
    
}