using GestionFacturas.Dominio;

namespace GestionFacturas.Web.Pages.Facturas.DisplayTemplates;

public class VisorFactura
{
    public VisorFactura()
    {
        
    }

    public VisorFactura(Factura factura)
    {
        Id = factura.Id;
        NumeroFactura = string.Format(factura.FormatoNumeroFactura, factura.SerieFactura, factura.NumeracionFactura);
        FechaEmisionFactura = factura.FechaEmisionFactura;
        FechaVencimientoFactura = factura.FechaVencimientoFactura;
        IdVendedor = factura.IdVendedor;

        VendedorNumeroIdentificacionFiscal = factura.VendedorNumeroIdentificacionFiscal;
        VendedorNombreOEmpresa = factura.VendedorNombreOEmpresa;
        VendedorDireccion = factura.VendedorDireccion;
        VendedorLocalidad = factura.VendedorLocalidad;
        VendedorProvincia = factura.VendedorProvincia;
        VendedorCodigoPostal = factura.VendedorCodigoPostal;


        IdComprador = factura.IdComprador;
        CompradorNumeroIdentificacionFiscal = factura.CompradorNumeroIdentificacionFiscal;
        CompradorNombreOEmpresa = factura.CompradorNombreOEmpresa;
        CompradorDireccion = factura.CompradorDireccion;
        CompradorLocalidad = factura.CompradorLocalidad;
        CompradorProvincia = factura.CompradorProvincia;
        CompradorCodigoPostal = factura.CompradorCodigoPostal;

        FormaPago = factura.FormaPago;
        FormaPagoDetalles = factura.FormaPagoDetalles;

        EstadoFactura = factura.EstadoFactura;
        Comentarios = factura.Comentarios;
        ComentariosPie = factura.ComentariosPie;
        ComentarioInterno = factura.ComentarioInterno;

        Titulo = factura.NumeroYEmpresaFactura();

        BaseImponible = factura.BaseImponible();

        ImporteImpuestos = factura.ImporteImpuestos();

        ImporteTotal = factura.ImporteTotal();

        Lineas = factura.Lineas.Select(m => new LineaVisorFactura(m)).ToList();

        EnviadaAHacienda = factura.EnviadaAHacienda;

        VerifactuCsv = factura.VerifactuCsv ?? string.Empty;
    }

    public int Id { get; set; }
    
    public string NumeroFactura { get; set; } = string.Empty;

    public DateTime FechaEmisionFactura { get; set; }
    public DateTime? FechaVencimientoFactura { get; set; }

    public int? IdVendedor { get; set; }
        
    public string VendedorNumeroIdentificacionFiscal { get; set; } = string.Empty;
    public string VendedorNombreOEmpresa { get; set; } = string.Empty;
    public string VendedorDireccion { get; set; } = string.Empty;
    public string VendedorLocalidad { get; set; } = string.Empty;
    public string VendedorProvincia { get; set; } = string.Empty;
    public string VendedorCodigoPostal { get; set; } = string.Empty;


    public int? IdComprador { get; set; }
    public string CompradorNumeroIdentificacionFiscal { get; set; } = string.Empty;
    public string CompradorNombreOEmpresa { get; set; } = string.Empty;
    public string CompradorDireccion { get; set; } = string.Empty;
    public string? CompradorLocalidad { get; set; } = string.Empty;
    public string? CompradorProvincia { get; set; } = string.Empty;
    public string? CompradorCodigoPostal { get; set; } = string.Empty;

    public FormaPagoEnum FormaPago { get; set; }
    public string FormaPagoDetalles { get; set; } = string.Empty;

    public ICollection<LineaVisorFactura> Lineas { get; set; } = new List<LineaVisorFactura>();
    
    public EstadoFacturaEnum EstadoFactura { get; set; }
    public string? Comentarios { get; set; } = string.Empty;
    public string? ComentariosPie { get; set; } = string.Empty;

    public string? ComentarioInterno { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public decimal BaseImponible { get; set; }

    public decimal ImporteImpuestos { get; set; }

    public decimal ImporteTotal { get; set; }

    public bool EnviadaAHacienda { get; set; }

    public string VerifactuCsv { get; set; } = string.Empty;

}