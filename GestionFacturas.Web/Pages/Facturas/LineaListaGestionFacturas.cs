using System.Text.Json.Serialization;
using GestionFacturas.Dominio;
using GestionFacturas.Dominio.Infra;

namespace GestionFacturas.Web.Pages.Facturas;

public class LineaListaGestionFacturas
{
    public int Id { get; set; }
    public string IdUsuario { get; set; } = string.Empty;

    public string SerieFactura { get; set; } = string.Empty;
    public int NumeracionFactura { get; set; }
    public string FormatoNumeroFactura { get; set; } = string.Empty;

    public string DescripcionPrimeraLinea { get; set; } = string.Empty;
    
    public string NumeroFactura { get { return string.Format(FormatoNumeroFactura, SerieFactura, NumeracionFactura); } }

    [JsonIgnore]
    public DateTime FechaEmisionFacturaDateTime { get; set; }
    public string FechaEmisionFactura => FechaEmisionFacturaDateTime.ToFechaCorta();
    public DateTime? FechaVencimientoFactura { get; set; }

    public int? IdComprador { get; set; }
    public string CompradorNombreOEmpresa { get; set; } = string.Empty;

    public string CompradorNombreComercial { get; set; } = string.Empty;

    public string CompradorNombre { get { return CompradorNombreComercial ?? CompradorNombreOEmpresa; } }

    public decimal BaseImponible { get; set; }
    public decimal Impuestos { get; set; }
    public decimal ImporteTotal {
        get { return BaseImponible + Impuestos; }
    }

    public EstadoFacturaEnum EstadoFactura { get; set; }

    public bool EnviadaAHacienda { get; set; }

}