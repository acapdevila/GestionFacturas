﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionFacturas.Modelos
{
    public class Factura
    {
        public Factura()
        {
            Lineas = new List<LineaFactura>();
        }
        
        public int Id { get; set; }
        public string IdUsuario { get; set; }
        
        public string SerieFactura { get; set; }
        public int NumeracionFactura { get; set; }
        public string FormatoNumeroFactura { get; set; }

        public string NumeroFactura { get { return string.Format(FormatoNumeroFactura ?? "", SerieFactura ?? "", NumeracionFactura); } }
               
        public DateTime FechaEmisionFactura { get; set; }
        public DateTime? FechaVencimientoFactura { get; set; }

        [Display(Name = "Forma de pago")]
        public FormaPagoEnum FormaPago { get; set; }

        [Display(Name = "Detalles forma pago")]
        public string FormaPagoDetalles { get; set; }

        public int? IdVendedor { get; set; }
        public string VendedorNumeroIdentificacionFiscal { get; set; }
        public string VendedorNombreOEmpresa { get; set; }
        public string VendedorDireccion { get; set; }
        public string VendedorLocalidad { get; set; }
        public string VendedorProvincia { get; set; }
        public string VendedorCodigoPostal { get; set; }
        public string VendedorEmail { get; set; }

        public int? IdComprador { get; set; }
        public string CompradorNumeroIdentificacionFiscal { get; set; }
        public string CompradorNombreOEmpresa { get; set; }
        public string CompradorDireccion { get; set; }
        public string CompradorLocalidad { get; set; }
        public string CompradorProvincia { get; set; }
        public string CompradorCodigoPostal { get; set; }

        public string CompradorEmail { get; set; }

        public virtual ICollection<LineaFactura> Lineas { get; set; }
        public virtual Usuario Usuario { get; set; }
        
        public EstadoFacturaEnum EstadoFactura { get; set; }
        public string Comentarios { get; set; }
        public string ComentariosPie { get; set; }
        public string ComentarioInterno { get; set; }

        public string NombreArchivoLogo { get; set; }
        public string NombreArchivoPlantillaInforme { get; set; }

        public string Titulo
        {
            get
            {
                return string.Format("Factura {0} {1}", NumeroFactura, CompradorNombreOEmpresa);
            }
        }

        public decimal BaseImponible()
        {
            if (!Lineas.Any()) return 0;

            return Lineas.Sum(m => m.PrecioXCantidad);

        }
        public decimal ImporteImpuestos()
        {
            if (!Lineas.Any()) return 0;

            return Lineas.Sum(m => m.PrecioXCantidad * m.PorcentajeImpuesto / 100);
        }
        public decimal ImporteTotal()
        {
            if (!Lineas.Any()) return 0;

            return Lineas.Sum(m => m.PrecioXCantidad + (m.PrecioXCantidad * m.PorcentajeImpuesto / 100));
        }

        public virtual Cliente Comprador { get; set; }
    }
    public enum EstadoFacturaEnum
    {
        Creada = 1,
        Enviada = 2,
        Cobrada = 3
    }

    public enum FormaPagoEnum
    {
        [Display(Name = @"Sin definir")]
        SinDefinir = 0,
        Transferencia = 1,
        Tarjeta = 2,
        Efectivo = 3,
        [Display(Name = @"Domiciliación")]
        Domiciliacion = 4
    }

    public class LineaFactura
    {
        public int Id { get; set; }
        public int IdFactura { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioXCantidad { get { return PrecioUnitario * Cantidad; } }
        public int PorcentajeImpuesto { get; set; }
        public decimal ImporteBruto { get { return PrecioXCantidad + ((PrecioXCantidad * PorcentajeImpuesto) /100); } }
        public virtual Factura Factura { get; set; }
    }

    public class LineaListaGestionFacturas
    {
        public int Id { get; set; }
        public string IdUsuario { get; set; }

        public string SerieFactura { get; set; }
        public int NumeracionFactura { get; set; }
        public string FormatoNumeroFactura { get; set; }

        public string NumeroFactura { get { return string.Format(FormatoNumeroFactura, SerieFactura, NumeracionFactura); } }

        public DateTime FechaEmisionFactura { get; set; }
        public DateTime? FechaVencimientoFactura { get; set; }

        public IEnumerable<string> ListaDescripciones { get; set; }

        public string Conceptos { get {
                return string.Join(", ", ListaDescripciones);
            } }

        public int? IdComprador { get; set; }
        public string CompradorNombreOEmpresa { get; set; }

        public decimal BaseImponible { get; set; }
        public decimal Impuestos { get; set; }
        public decimal ImporteTotal { get; set; }

        public EstadoFacturaEnum EstadoFactura { get; set; }

    }

    public class EditorFactura
    {
        public int DimensionMaximaLogo { get { return 300; } }

        public EditorFactura()
        {
            Lineas = new List<EditorLineaFactura>();
        }

        public int Id { get; set; }
        public string IdUsuario { get; set; }

        [Required]
        [Display(Name="Serie")]
        [StringLength(50)]
        public string SerieFactura { get; set; }

        [Required]
        [Display(Name = "Número")]
        public int NumeracionFactura { get; set; }

        [Required]
        [StringLength(50)]
        public string FormatoNumeroFactura { get; set; }

        public string NumeroFactura { get { return string.Format(FormatoNumeroFactura, SerieFactura, NumeracionFactura); } }

        [Required]
        [Display(Name = "Fecha emisión")]
        public DateTime FechaEmisionFactura { get; set; }

        [Display(Name = "Fecha vencimiento")]
        public DateTime? FechaVencimientoFactura { get; set; }
        
        [Display(Name = "Logo")]
        [StringLength(50)]
        public string NombreArchivoLogo { get; set; }
        
        [Display(Name = "Forma de pago")]
        public FormaPagoEnum FormaPago { get; set; }

        [Display(Name = "Detalles forma pago")]
        [StringLength(50)]
        public string FormaPagoDetalles { get; set; }

        [Display(Name = "Número de referencia")]
        public int? IdVendedor { get; set; }

        [Display(Name = "Identificación fiscal")]
        [StringLength(50)]
        public string VendedorNumeroIdentificacionFiscal { get; set; }

        [Display(Name = "Nombre o empresa")]
        [StringLength(50)]
        public string VendedorNombreOEmpresa { get; set; }

        [Display(Name = "Dirección")]
        [StringLength(50)]
        public string VendedorDireccion { get; set; }

        [Display(Name = "Municipio")]
        [StringLength(50)]
        public string VendedorLocalidad { get; set; }

        [Display(Name = "Provincia")]
        [StringLength(50)]
        public string VendedorProvincia { get; set; }

        [Display(Name = "Código postal")]
        [StringLength(10)]
        public string VendedorCodigoPostal { get; set; }

        [EmailAddress]
        [Display(Name = "E-mail")]
        [StringLength(50)]
        public string VendedorEmail { get; set; }

        [Display(Name = "Número de referencia")]
        public int? IdComprador { get; set; }

        [Display(Name = "Identificación fiscal")]
        [StringLength(50)]
        public string CompradorNumeroIdentificacionFiscal { get; set; }

        [Display(Name = "Nombre o empresa")]
        [StringLength(50)]
        public string CompradorNombreOEmpresa { get; set; }

        [Display(Name = "Dirección")]
        [StringLength(50)]
        public string CompradorDireccion { get; set; }

        [Display(Name = "Municipio")]
        [StringLength(50)]
        public string CompradorLocalidad { get; set; }

        [Display(Name = "Provincia")]
        [StringLength(50)]
        public string CompradorProvincia { get; set; }

        [Display(Name = "Código postal")]
        [StringLength(10)]
        public string CompradorCodigoPostal { get; set; }

        [EmailAddress]
        [Display(Name = "E-mail")]
        [StringLength(50)]
        public string CompradorEmail { get; set; }



        public virtual ICollection<EditorLineaFactura> Lineas { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "Estado")]
        public EstadoFacturaEnum EstadoFactura { get; set; }

        [StringLength(250)]
        public string Comentarios { get; set; }

        [Display(Name = "Pie")]
        [StringLength(500)]
        public string ComentariosPie { get; set; }

        [Display(Name = "Nota interna")]
        [StringLength(250)]
        public string ComentarioInterno { get; set; }


        public int PorcentajeIvaPorDefecto { get; set; }

        public void BorrarLineasFactura()
        {
            while (Lineas.Any())
            {
                var linea = Lineas.First();
                Lineas.Remove(linea);
            }
        }
    }

    public class VisorFactura
    {
        public VisorFactura()
        {
            Lineas = new List<LineaVisorFactura>();
        }

        public int Id { get; set; }
        public string IdUsuario { get; set; }

        public string SerieFactura { get; set; }
        public int NumeracionFactura { get; set; }
        public string FormatoNumeroFactura { get; set; }

        public string NumeroFactura { get { return string.Format(FormatoNumeroFactura, SerieFactura, NumeracionFactura); } }

        public DateTime FechaEmisionFactura { get; set; }
        public DateTime? FechaVencimientoFactura { get; set; }

        public int? IdVendedor { get; set; }
        
        public string VendedorNumeroIdentificacionFiscal { get; set; }
        public string VendedorNombreOEmpresa { get; set; }
        public string VendedorDireccion { get; set; }
        public string VendedorLocalidad { get; set; }
        public string VendedorProvincia { get; set; }
        public string VendedorCodigoPostal { get; set; }

     
        public int? IdComprador { get; set; }
        public string CompradorNumeroIdentificacionFiscal { get; set; }
        public string CompradorNombreOEmpresa { get; set; }
        public string CompradorDireccion { get; set; }
        public string CompradorLocalidad { get; set; }
        public string CompradorProvincia { get; set; }
        public string CompradorCodigoPostal { get; set; }

        public FormaPagoEnum FormaPago { get; set; }
        public string FormaPagoDetalles { get; set; }

        public virtual ICollection<LineaVisorFactura> Lineas { get; set; }
        public virtual Usuario Usuario { get; set; }

        public EstadoFacturaEnum EstadoFactura { get; set; }
        public string Comentarios { get; set; }
        public string ComentariosPie { get; set; }

        public string ComentarioInterno { get; set; }

        public string Titulo { get; set; }
       
        public decimal BaseImponible
        {
            get {
                if (!Lineas.Any()) return 0;

                return Lineas.Sum(m => m.PrecioXCantidad);
            }
           

        }
        public decimal ImporteImpuestos
        {
            get {
                if (!Lineas.Any()) return 0;

                return Lineas.Sum(m => m.PrecioXCantidad * m.PorcentajeImpuesto / 100);
            }
           
        }
        public decimal ImporteTotal
        {
            get {
                if (!Lineas.Any()) return 0;

                return Lineas.Sum(m => m.PrecioXCantidad + (m.PrecioXCantidad * m.PorcentajeImpuesto / 100));
            }
            
        }

        public void BorrarLineasFactura()
        {
            while (Lineas.Any())
            {
                var linea = Lineas.First();
                Lineas.Remove(linea);
            }
        }
    }

    public class EditorLineaFactura
    {
        public EditorLineaFactura(int iva)
        {
            Cantidad = 1;
            PorcentajeImpuesto = iva;
        }
        public EditorLineaFactura()
        {
        }

        
        public int Id { get; set; }
        public int IdFactura { get; set; }

        [Display(Name = "Concepto")]
        public string Descripcion { get; set; }

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Precio unitario")]
        public decimal PrecioUnitario { get; set; }

        [Display(Name = "% IVA")]
        public int PorcentajeImpuesto { get; set; }

        public bool EstaMarcadoParaEliminar { get; set; }
    }

    public class LineaVisorFactura
    {
        public int Id { get; set; }
        public int IdFactura { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioXCantidad { get { return PrecioUnitario * Cantidad; } }
        public int PorcentajeImpuesto { get; set; }
        public decimal ImporteBruto { get { return PrecioXCantidad + ((PrecioXCantidad * PorcentajeImpuesto) / 100); } }
     }


}
