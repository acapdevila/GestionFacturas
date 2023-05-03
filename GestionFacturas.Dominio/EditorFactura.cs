﻿using GestionFacturas.Dominio.Clientes;
using GestionFacturas.Dominio.Infra;
using System.ComponentModel.DataAnnotations;
using Omu.ValueInjecter;

namespace GestionFacturas.Dominio;

public class EditorFactura
{
    public EditorFactura()
    {
    }

    public EditorFactura(Factura factura)
    {
        this.InjectFrom(factura);

        this.PorcentajeIvaPorDefecto = 21;

        this.BorrarLineasFactura();

        foreach (var lineaFactura in factura.Lineas)
        {
            var lineaEditor = new EditorLineaFactura(lineaFactura);
            this.Lineas.Add(lineaEditor);
        }

        this.CompradorDireccion1 = factura.CompradorDireccion1();
        this.CompradorDireccion2 = factura.CompradorDireccion2();
        this.FechaEmisionFactura = factura.FechaEmisionFactura.ToInputDate();
        this.FechaVencimientoFactura = factura.FechaVencimientoFactura?.ToInputDate();
    }

    public int Id { get; set; }
    public string IdUsuario { get; set; } = string.Empty;

    [Required]
    [Display(Name="Serie")]
    [StringLength(50)]
    public string SerieFactura { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Número")]
    public int NumeracionFactura { get; set; } 

    [Required]
    [StringLength(50)]
    public string FormatoNumeroFactura { get; set; } = string.Empty;

    public string NumeroFactura { get { return string.Format(FormatoNumeroFactura, SerieFactura, NumeracionFactura); } }

    [Required]
    [Display(Name = "Fecha emisión")]
    public string FechaEmisionFactura { get; set; } = string.Empty;

    [Display(Name = "Fecha vencimiento")]
    public string? FechaVencimientoFactura { get; set; }

    [Required]
    [Display(Name = "Plantilla")]
    [StringLength(50)]
    public string NombreArchivoPlantillaInforme { get; set; } = string.Empty;

    [Display(Name = "Forma de pago")]
    public FormaPagoEnum FormaPago { get; set; }

    [Display(Name = "Detalles forma pago")]
    [StringLength(50)]
    public string FormaPagoDetalles { get; set; } = string.Empty;

    [Display(Name = "Número de referencia")]
    public int? IdVendedor { get; set; }

    [Display(Name = "Identificación fiscal")]
    [StringLength(50)]
    public string VendedorNumeroIdentificacionFiscal { get; set; } = string.Empty;

    [Display(Name = "Nombre o empresa")]
    [StringLength(50)]
    public string VendedorNombreOEmpresa { get; set; } = string.Empty;

    [Display(Name = "Dirección")]
    [StringLength(50)]
    public string VendedorDireccion { get; set; } = string.Empty;

    [Display(Name = "Municipio")]
    [StringLength(50)]
    public string VendedorLocalidad { get; set; } = string.Empty;

    [Display(Name = "Provincia")]
    [StringLength(50)]
    public string VendedorProvincia { get; set; } = string.Empty;

    [Display(Name = "Código postal")]
    [StringLength(10)]
    public string VendedorCodigoPostal { get; set; } = string.Empty;

    [EmailAddress]
    [Display(Name = "E-mail")]
    [StringLength(256)]
    public string? VendedorEmail { get; set; } = string.Empty;

    [Display(Name = "Número de referencia")]
    public int? IdComprador { get; set; }

    [Display(Name = "Identificación fiscal")]
    [StringLength(50)]
    public string CompradorNumeroIdentificacionFiscal { get; set; } = string.Empty;

    [Display(Name = "Nombre o empresa")]
    [StringLength(128)]
    public string CompradorNombreOEmpresa { get; set; } = string.Empty;

    public string CompradorDireccion {
        get { return CompradorDireccion1 + (string.IsNullOrEmpty(CompradorDireccion2) ? string.Empty : Environment.NewLine + CompradorDireccion2);  }
    }

    [Display(Name = "Dirección 1")]
    [StringLength(64)]
    public string CompradorDireccion1 { get; set; } = string.Empty;

    [Display(Name = "Dirección 2")]
    [StringLength(64)]
    public string? CompradorDireccion2 { get; set; } = string.Empty;

    [Display(Name = "Municipio")]
    [StringLength(50)]
    public string? CompradorLocalidad { get; set; } = string.Empty;

    [Display(Name = "Provincia")]
    [StringLength(50)]
    public string? CompradorProvincia { get; set; } = string.Empty;

    [Display(Name = "Código postal")]
    [StringLength(10)]
    public string? CompradorCodigoPostal { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    [StringLength(256)]
    public string? CompradorEmail { get; set; } = string.Empty;


    public List<EditorLineaFactura> Lineas { get; set; } = new ();
    
    [Display(Name = "Estado")]
    public EstadoFacturaEnum EstadoFactura { get; set; }

    [StringLength(250)]
    public string? Comentarios { get; set; } = string.Empty;

    [Display(Name = "Pie")]
    [StringLength(800)]
    public string? ComentariosPie { get; set; } = string.Empty;

    [Display(Name = "Nota interna")]
    [StringLength(250)]
    public string? ComentarioInterno { get; set; } = string.Empty;


    public int PorcentajeIvaPorDefecto { get; set; }

    public void BorrarLineasFactura()
    {
        while (Lineas.Any())
        {
            var linea = Lineas.First();
            Lineas.Remove(linea);
        }
    }

    public void AsignarDatosCliente(Cliente cliente)
    {
        IdComprador = cliente.Id;
        CompradorCodigoPostal = cliente.CodigoPostal ?? string.Empty;
        CompradorDireccion1 = cliente.Direccion1();
        CompradorDireccion2 = cliente.Direccion2();
        CompradorEmail = cliente.Email ?? string.Empty;
        CompradorLocalidad = cliente.Localidad ?? string.Empty;
        CompradorNombreOEmpresa = cliente.NombreOEmpresa;
        CompradorNumeroIdentificacionFiscal = cliente.NumeroIdentificacionFiscal;
        CompradorProvincia = cliente.Provincia ?? string.Empty;
    }

    public EditorFactura GenerarNuevoEditorFacturaDuplicado(
                    Factura factura,
                    Factura ultimaFacturaSerie,
                    Cliente cliente)
    {
        var editor = new EditorFactura
        {
            SerieFactura = factura.SerieFactura,
            NumeracionFactura = ultimaFacturaSerie!.NumeracionFactura + 1,
            FormatoNumeroFactura = ultimaFacturaSerie.FormatoNumeroFactura,
            FechaEmisionFactura = DateTime.Today.ToInputDate(),
            NombreArchivoPlantillaInforme = factura.NombreArchivoPlantillaInforme,
            PorcentajeIvaPorDefecto = PorcentajeIvaPorDefecto,
            FormaPago = factura.FormaPago,
            FormaPagoDetalles = factura.FormaPagoDetalles,
            ComentariosPie = factura.ComentariosPie,
            EstadoFactura = EstadoFacturaEnum.Borrador,
            IdVendedor = factura.IdVendedor,
            VendedorCodigoPostal = factura.VendedorCodigoPostal,
            VendedorDireccion = factura.VendedorDireccion,
            VendedorEmail = factura.VendedorEmail,
            VendedorLocalidad = factura.VendedorLocalidad,
            VendedorNombreOEmpresa = factura.VendedorNombreOEmpresa,
            VendedorNumeroIdentificacionFiscal = factura.VendedorNumeroIdentificacionFiscal,
            VendedorProvincia = factura.VendedorProvincia,
            Lineas = new List<EditorLineaFactura>()
        };


        foreach (var linea in factura.Lineas)
        {
            editor.Lineas.Add(new EditorLineaFactura
            {
                PorcentajeImpuesto = linea.PorcentajeImpuesto,
                Cantidad = linea.Cantidad,
                Descripcion = linea.Descripcion,
                PrecioUnitario = linea.PrecioUnitario
            });
        }
       
        editor.AsignarDatosCliente(cliente!);

        return editor;
    }

    public EditorFactura ObtenerEditorFacturaParaCrearNuevaFactura(Factura? ultimaFacturaCreada, Cliente? cliente)
    {
        EditorFactura editor;
        
        if (ultimaFacturaCreada is null)
        {
            editor = new EditorFactura
            {
                SerieFactura = string.Empty,
                NumeracionFactura = 1,
                FormatoNumeroFactura = "{0}{1:1000}",
                FechaEmisionFactura = DateTime.Today.ToInputDate(),
                PorcentajeIvaPorDefecto = PorcentajeIvaPorDefecto,
                FormaPago = FormaPagoEnum.Transferencia,
                EstadoFactura = EstadoFacturaEnum.Borrador,

                Lineas = new List<EditorLineaFactura> {
                            new EditorLineaFactura {
                                    Cantidad = 1,
                                    PorcentajeImpuesto = PorcentajeIvaPorDefecto
                            }
                      }
            };
        }
        else
        {
            editor = new EditorFactura
            {
                SerieFactura = ultimaFacturaCreada.SerieFactura,
                NumeracionFactura = ultimaFacturaCreada.NumeracionFactura + 1,
                FormatoNumeroFactura = ultimaFacturaCreada.FormatoNumeroFactura,
                FechaEmisionFactura = DateTime.Today.ToInputDate(),
                NombreArchivoPlantillaInforme = ultimaFacturaCreada.NombreArchivoPlantillaInforme,
                PorcentajeIvaPorDefecto = PorcentajeIvaPorDefecto,
                FormaPago = ultimaFacturaCreada.FormaPago,
                FormaPagoDetalles = ultimaFacturaCreada.FormaPagoDetalles,
                ComentariosPie = ultimaFacturaCreada.ComentariosPie,
                EstadoFactura = EstadoFacturaEnum.Borrador,
                IdVendedor = ultimaFacturaCreada.IdVendedor,
                VendedorCodigoPostal = ultimaFacturaCreada.VendedorCodigoPostal,
                VendedorDireccion = ultimaFacturaCreada.VendedorDireccion,
                VendedorEmail = ultimaFacturaCreada.VendedorEmail,
                VendedorLocalidad = ultimaFacturaCreada.VendedorLocalidad,
                VendedorNombreOEmpresa = ultimaFacturaCreada.VendedorNombreOEmpresa,
                VendedorNumeroIdentificacionFiscal = ultimaFacturaCreada.VendedorNumeroIdentificacionFiscal,
                VendedorProvincia = ultimaFacturaCreada.VendedorProvincia,

                Lineas = new List<EditorLineaFactura> {
                            new EditorLineaFactura {
                                    Cantidad = 1,
                                    PorcentajeImpuesto = PorcentajeIvaPorDefecto
                            }
                      }
            };
        }

        if (cliente is not null)
        {
           editor.AsignarDatosCliente(cliente);
        }

        return editor;
    }


}