using System.Drawing;
using GestionFacturas.Aplicacion;
using GestionFacturas.Dominio;
using GestionFacturas.Dominio.Infra;
using Microsoft.Reporting.NETCore;

namespace GestionFacturas.Web.Pages.Facturas
{
    public static class GeneraLocalReportFactura
    {
        
        public static LocalReport GenerarInformeLocalFactura(Factura factura, MemoryStream qr, string webRootPath)
        {
            var plantillaRdlc = string.IsNullOrEmpty(factura.NombreArchivoPlantillaInforme)
                                                    ? "Factura.rdlc"
                                                    : factura.NombreArchivoPlantillaInforme;

            var rutaReport = Path.Combine(webRootPath, "informes", plantillaRdlc);

            var informeLocal = new LocalReport
            {
                ReportPath = rutaReport,
                EnableExternalImages = true
            };
            
            var datasetFactura = ConvertirADataSet(factura, qr);

            informeLocal.DataSources.Add(new ReportDataSource("Facturas", datasetFactura.Tables[0]));
            informeLocal.DataSources.Add(new ReportDataSource("Lineas", datasetFactura.Tables[1]));

            

            return informeLocal;
        }

        private static DataSetFactura ConvertirADataSet(Factura factura, MemoryStream qr)
        {
            var datasetFactura = new DataSetFactura();
            
            var filaDatasetFactura = datasetFactura.Facturas.NewFacturasRow();

            filaDatasetFactura.CopiarFactura(factura);
            filaDatasetFactura.QrBytes = qr.ToArray();

            datasetFactura.Facturas.AddFacturasRow(filaDatasetFactura);

            foreach (var linea in factura.Lineas)
            {
                var filaDatasetLineaFactura = datasetFactura.FacturasLineas.NewFacturasLineasRow();

                filaDatasetLineaFactura.Id = linea.Id;
                filaDatasetLineaFactura.IdFactura = linea.IdFactura;
                filaDatasetLineaFactura.Descripcion = linea.Descripcion;
                filaDatasetLineaFactura.Cantidad = linea.Cantidad;
                filaDatasetLineaFactura.PrecioUnitario = linea.PrecioUnitario;
                filaDatasetLineaFactura.PorcentajeImpuesto = linea.PorcentajeImpuesto;

                datasetFactura.FacturasLineas.AddFacturasLineasRow(filaDatasetLineaFactura);
            }

            return datasetFactura;
        }

        public static void CopiarFactura(this DataSetFactura.FacturasRow fila, Factura factura)
        {
            fila.Id = factura.Id;
            fila.RutaLogo = "/Content/Logos/LogoGF.jpg";

            fila.NumeroFactura = factura.NumeroFactura;
            fila.FechaEmisionFactura = factura.FechaEmisionFactura;

            if (factura.FechaVencimientoFactura.HasValue)
                fila.FechaVencimientoFactura = factura.FechaVencimientoFactura.Value;
            else
                fila.SetFechaVencimientoFacturaNull();

            fila.VendedorNumeroIdentificacionFiscal = factura.VendedorNumeroIdentificacionFiscal;
            fila.VendedorNombreOEmpresa = factura.VendedorNombreOEmpresa;
            fila.VendedorDireccion = factura.VendedorDireccion;
            fila.VendedorLocalidad = factura.VendedorLocalidad;
            fila.VendedorProvincia = factura.VendedorProvincia;
            fila.VendedorCodigoPostal = factura.VendedorCodigoPostal;

            fila.CompradorNumeroIdentificacionFiscal = factura.CompradorNumeroIdentificacionFiscal;
            fila.CompradorNombreOEmpresa = factura.CompradorNombreOEmpresa;
            fila.CompradorDireccion = factura.CompradorDireccion;
            fila.CompradorLocalidad = factura.CompradorLocalidad;
            fila.CompradorProvincia = factura.CompradorProvincia;
            fila.CompradorCodigoPostal = factura.CompradorCodigoPostal;

            fila.Comentarios = factura.Comentarios;
            fila.ComentariosPie = factura.ComentariosPie;

            fila.FormaPago = factura.FormaPago.ObtenerNombreAtributoDisplay();
            fila.FormaPagoDetalles = factura.FormaPagoDetalles;

            fila.BaseImponible = factura.BaseImponible();
            fila.ImporteImpuestos = factura.ImporteImpuestos();
            fila.ImporteTotal = factura.ImporteTotal();
        }
    }
}
