using CSharpFunctionalExtensions;

namespace GestionFacturas.Dominio.Clientes
{

    public class Cliente : Entity<int>
    {
        public string NumeroIdentificacionFiscal { get; set; } = string.Empty;

        public string NombreOEmpresa { get; set; } = string.Empty;

        public string NombreComercial { get; set; } = string.Empty;

        public string? Direccion { get; set; } = string.Empty;

        private string[] LineasDireccion() => Direccion?.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None) ?? new List<string>().ToArray();

        public string Direccion1() => LineasDireccion().FirstOrDefault() ?? string.Empty;

        public string Direccion2() => LineasDireccion().Length > 1 ? LineasDireccion()[1] : string.Empty;

        public string? Localidad { get; set; } = string.Empty;

        public string? Provincia { get; set; } = string.Empty;

        public string? CodigoPostal { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? PersonaContacto { get; set; } 

        public string? ComentarioInterno { get; set; }
        
      
        public string CodigoPaisIso2 { get; set; } = string.Empty;

        public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    }
}
