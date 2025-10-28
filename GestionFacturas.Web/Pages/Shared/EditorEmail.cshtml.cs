using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionFacturas.Web.Pages.Shared;

public class EditorEmail
{
    [Display(Name = @"DisplayName de e-mail")]
    [Required(ErrorMessage = @"Escribe el nombre del remitente")]
    public string DisplayName { get; set; } = string.Empty;


    [Display(Name = @"De")]
    [Required(ErrorMessage = @"Escribe la dirección de e-mail del remitente")]
    [RegularExpression(
        @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$",
        ErrorMessage = "Verifica tu e-mail.")]
    public string Remitente { get; set; } = string.Empty;

    [Display(Name = @"Destinatarios separados por ;")]
    [Required(ErrorMessage = @"Escribe al menos la dirección de e-mail de un destinatario")]
    public string Destinatarios { get; set; } = string.Empty;

    [Display(Name = @"Asunto")]
    [Required(ErrorMessage = @"Escribe el asunto")]
    public string Asunto { get; set; } = string.Empty;

    [Display(Name = @"Mensaje")]
    public string ContenidoTexto { get; set; } = string.Empty;

    public List<SelectListItem> EmailRemitentes { get; set; } = new();

    public List<SelectListItem> NombresRemitentes { get; set; } = new();


    [Display(Name = @"Adjuntar archivos")]
    public List<IFormFile>? ArchivosAdjuntos { get; set; } = default!;


}