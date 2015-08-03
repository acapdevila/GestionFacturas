﻿using System;
using System.Collections.Generic;
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
           
        public string SerieFactura { get; set; }
        public int NumeracionFactura { get; set; }
        public string FormatoNumeroFactura { get; set; }

        public string NumeroFactura { get { return string.Format(FormatoNumeroFactura, SerieFactura, NumeracionFactura); } }
               
        public DateTime FechaEmisionFactura { get; set; }
        public DateTime FechaVencimientoFactura { get; set; }      

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

        public virtual ICollection<LineaFactura> Lineas { get; set; }
        
        public EstadoFacturaEnum EstadoFactura { get; set; }
        public string Comentarios { get; set; }
        public string ComentariosPie { get; set; }


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
    }
     public enum EstadoFacturaEnum
    {
        Borrador = 0,
        Creada = 1,
        Enviada = 2,
        Cobrada = 3,
        Anulada = 4
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


   
}
