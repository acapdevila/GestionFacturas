using System.ComponentModel.DataAnnotations;
using GestionFacturas.Dominio.Clientes;

namespace GestionFacturas.Web.Pages.Clientes;

public class EditorClienteVm
{
    public EditorClienteVm()
    {
        
    }

    public EditorClienteVm(Cliente cliente)
    {
        Id = cliente.Id;
        NumeroIdentificacionFiscal = cliente.NumeroIdentificacionFiscal;
        NombreOEmpresa = cliente.NombreOEmpresa;
        NombreComercial = cliente.NombreComercial;
        Direccion1 = cliente.Direccion1();
        Direccion2 = cliente.Direccion2();
        Localidad = cliente.Localidad;
        Provincia = cliente.Provincia;
        CodigoPostal = cliente.CodigoPostal;
        Email = cliente.Email;
        PersonaContacto = cliente.PersonaContacto;
        ComentarioInterno = cliente.ComentarioInterno;
        CodigoPaisIso2 = cliente.CodigoPaisIso2;

    }
    public int Id { get; set; }

    [Display(Name = "NIF")]
    public string NumeroIdentificacionFiscal { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Nombre o Empresa")]
    public string NombreOEmpresa { get; set; } = string.Empty;

    [Display(Name = "Nombre comercial")]
    public string NombreComercial { get; set; } = string.Empty;
    
    [Display(Name = "Dirección 1")]
    [StringLength(64)]
    public string Direccion1 { get; set; } = string.Empty;

    [Display(Name = "Dirección 2")]
    [StringLength(64)]
    public string? Direccion2 { get; set; } = string.Empty;

    [Display(Name = "Municipio")]
    public string? Localidad { get; set; } = string.Empty;

    [Display(Name = "Provincia")]
    public string? Provincia { get; set; } = string.Empty;


    [Display(Name = "Código postal")]
    public string? CodigoPostal { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    public string? Email { get; set; } = string.Empty;

    [Display(Name = "Persona de contacto")]
    public string? PersonaContacto { get; set; } = string.Empty;

    [Display(Name = "Nota interna")]
    public string? ComentarioInterno { get; set; } = string.Empty;

    [Display(Name = "Código país ISO (2)")]
    public string CodigoPaisIso2 { get; set; } = "es";
}