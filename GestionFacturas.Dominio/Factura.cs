using GestionFacturas.Dominio.Clientes;
using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;

namespace GestionFacturas.Dominio
{
    public class Factura
    {
        public Factura()
        {
            Lineas = new List<LineaFactura>();
        }
        
        public int Id { get; set; }
        public string IdUsuario { get; set; } = string.Empty;

        public string SerieFactura { get; set; } = string.Empty;
        public int NumeracionFactura { get; set; }
        public string FormatoNumeroFactura { get; set; } = string.Empty;

        public string NumeroFactura { get { return string.Format(FormatoNumeroFactura ?? "", SerieFactura ?? "", NumeracionFactura); } }
               
        public DateTime FechaEmisionFactura { get; set; }
        public DateTime? FechaVencimientoFactura { get; set; }

        [Display(Name = "Forma de pago")]
        public FormaPagoEnum FormaPago { get; set; }

        [Display(Name = "Detalles forma pago")]
        public string FormaPagoDetalles { get; set; } = string.Empty;

        public int? IdVendedor { get; set; }
        public string VendedorNumeroIdentificacionFiscal { get; set; } = string.Empty;
        public string VendedorNombreOEmpresa { get; set; } = string.Empty;
        public string VendedorDireccion { get; set; } = string.Empty;
        public string VendedorLocalidad { get; set; } = string.Empty;
        public string VendedorProvincia { get; set; } = string.Empty;
        public string VendedorCodigoPostal { get; set; } = string.Empty;
        public string? VendedorEmail { get; set; } = string.Empty;

        public string DescripcionPrimeraLinea { get; set; } = string.Empty; 

        public int? IdComprador { get; set; }
        public string CompradorNumeroIdentificacionFiscal { get; set; } = string.Empty;
        public string CompradorNombreOEmpresa { get; set; } = string.Empty;
        public string CompradorDireccion { get; set; } = string.Empty;

        private string[] LineasCompradorDireccion()
            => CompradorDireccion.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

        public string CompradorDireccion1()
            => LineasCompradorDireccion().FirstOrDefault() ?? string.Empty;

        public string CompradorDireccion2() => 
            LineasCompradorDireccion().Length > 1 ? LineasCompradorDireccion()[1] : string.Empty;

        public string? CompradorLocalidad { get; set; } = string.Empty;
        public string? CompradorProvincia { get; set; } = string.Empty;
        public string? CompradorCodigoPostal { get; set; } = string.Empty;

        public string? CompradorEmail { get; set; } = string.Empty;

        public virtual ICollection<LineaFactura> Lineas { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
        
        public EstadoFacturaEnum EstadoFactura { get; set; }
        public string? Comentarios { get; set; } = string.Empty;
        public string? ComentariosPie { get; set; } = string.Empty;
        public string? ComentarioInterno { get; set; } = string.Empty;

        public string NombreArchivoPlantillaInforme { get; set; } = string.Empty;

        public string? VerifactuCsv { get; set; }

        public bool EnviadaAHacienda { get; set;  }
        
        public string NumeroYEmpresaFactura() =>string.Format("Factura {0} {1}", NumeroFactura, CompradorNombreOEmpresa);
           

        public decimal BaseImponible()  
        {
            if (!Lineas.Any()) return 0;

            return Lineas.Sum(m => m.PrecioXCantidad);
           
        }

        public decimal ImporteImpuestos()
        {
            if (!Lineas.Any()) return 0;

            return Lineas.Sum(m => m.ImporteImpuesto);
            
        }

        public decimal ImporteTotal()
        {
            if (!Lineas.Any()) return 0;

            return BaseImponible() + ImporteImpuestos();
            
        }

        public virtual Cliente Comprador { get; set; } = new();

        public Result MarcarFacturaComoEnviadaAHacienda()
        {
            if(EnviadaAHacienda)
                return Result.Failure("Ya se ha enviado a hacienda previamente");
            
            this.EnviadaAHacienda = true;

            return Result.Success();
        }

        public void GuardarCsvDevueltoPorHacienda(string csv)
        {
            this.VerifactuCsv = csv;
        }
    }
}
