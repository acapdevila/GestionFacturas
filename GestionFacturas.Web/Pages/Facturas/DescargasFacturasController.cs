using GestionFacturas.Aplicacion;
using GestionFacturas.Dominio;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Extensions;
using GestionFacturas.AccesoDatosSql;
using GestionFacturas.Dominio.Infra;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using DocumentFormat.OpenXml.EMMA;

namespace GestionFacturas.Web.Pages.Facturas
{
    public class DescargasFacturasController : Controller
    {
        private readonly SqlDb _db;
        private readonly IWebHostEnvironment _env;
        private readonly IVerifactuServicio _verifactu;

        public DescargasFacturasController(IWebHostEnvironment env, SqlDb db, IVerifactuServicio verifactu)
        {
            _env = env;
            _db = db;
            _verifactu = verifactu;
        }

        public async Task<ActionResult> DescargarZip(GridParamsFacturas gridParams)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var facturas =
                await _db.Facturas
                    .AsNoTracking()
                    .FiltrarPorParametros(gridParams)
                    .Select(m => new LineaListaGestionFacturas
                    {
                        Id = m.Id,
                        IdUsuario = m.IdUsuario,
                        IdComprador = m.IdComprador,
                        FormatoNumeroFactura = m.FormatoNumeroFactura,
                        NumeracionFactura = m.NumeracionFactura,
                        SerieFactura = m.SerieFactura,
                        FechaEmisionFacturaDateTime = m.FechaEmisionFactura,
                        FechaVencimientoFactura = m.FechaVencimientoFactura,
                        EstadoFactura = m.EstadoFactura,
                        BaseImponible = m.Lineas.Sum(l => (decimal?)(l.PrecioUnitario * l.Cantidad)) ?? 0,
                        Impuestos = m.Lineas.Sum(l => (decimal?)Math.Round((l.PrecioUnitario * l.Cantidad * l.PorcentajeImpuesto / 100), 2)) ?? 0,
                        CompradorNombreOEmpresa = m.CompradorNombreOEmpresa,
                        CompradorNombreComercial = m.Comprador.NombreComercial,
                        DescripcionPrimeraLinea = m.DescripcionPrimeraLinea
                    })
                    .OrderBy_OrdenarPor(gridParams.Orden)
                    .ToListAsync();
            

            var archivoZip = await GenerarZip(facturas);

            var nombreArchivoZip = $"Facturas_desde_{gridParams.Desde}_hasta_{gridParams.Hasta}.zip";

            archivoZip.Position = 0;
            HttpContext.Response.Headers.Append("content-disposition", "attachment; filename=" + nombreArchivoZip);
            return File(archivoZip, "application/zip");

        }

        private async Task<MemoryStream> GenerarZip(IEnumerable<LineaListaGestionFacturas> listaGestionFacturas)
        {
            var archivoZip = new MemoryStream();

            using (var zip = new ZipArchive(archivoZip, ZipArchiveMode.Create, true))
            {
                foreach (var itemFactura in listaGestionFacturas)
                {
                    var factura = await _db.Facturas
                        .Include(m => m.Lineas)
                        .FirstAsync(m => m.Id == itemFactura.Id);

                    var qr = _verifactu.ObtenerQr(factura);
                    
                    var informeLocal = GeneraLocalReportFactura.GenerarInformeLocalFactura(factura, qr.Value, _env.WebRootPath);

                    byte[] pdf = informeLocal.Render("PDF");
                    var nombrePdf = factura.NumeroYEmpresaFactura()
                        .Replace(":", " ")
                        .Replace("·", "")
                        .Replace("€", "")
                        .Replace("/", "-")
                        .EliminarDiacriticos() + ".pdf";

                    var entry = zip.CreateEntry(nombrePdf, CompressionLevel.Optimal);
                    using (var entryStream = entry.Open())
                    {
                        await entryStream.WriteAsync(pdf, 0, pdf.Length);
                    }
                }
            }

            archivoZip.Position = 0;
            return archivoZip;
        }

        public async Task<ActionResult> DescargarExcel(GridParamsFacturas gridParams)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var facturas =
                await _db.Facturas
                .AsNoTracking()
                .FiltrarPorParametros(gridParams)
                .Select(m => new LineaListaGestionFacturas
                {
                    Id = m.Id,
                    IdUsuario = m.IdUsuario,
                    IdComprador = m.IdComprador,
                    FormatoNumeroFactura = m.FormatoNumeroFactura,
                    NumeracionFactura = m.NumeracionFactura,
                    SerieFactura = m.SerieFactura,
                    FechaEmisionFacturaDateTime = m.FechaEmisionFactura,
                    FechaVencimientoFactura = m.FechaVencimientoFactura,
                    EstadoFactura = m.EstadoFactura,
                    BaseImponible = m.Lineas.Sum(l => (decimal?)(l.PrecioUnitario * l.Cantidad)) ?? 0,
                    Impuestos = m.Lineas.Sum(l => (decimal?)Math.Round((l.PrecioUnitario * l.Cantidad * l.PorcentajeImpuesto / 100), 2)) ?? 0,
                    CompradorNombreOEmpresa = m.CompradorNombreOEmpresa,
                    DescripcionPrimeraLinea = m.DescripcionPrimeraLinea,
                    CompradorNombreComercial = m.Comprador.NombreComercial
                })
                .OrderBy_OrdenarPor(gridParams.Orden)
                .ToListAsync();


            var workbook = ServicioExcel.GenerarExcelFactura(gridParams, facturas);

            var nombreArchivoExcel = $"Facturacion_desde_{gridParams.Desde}_hasta_{gridParams.Hasta}.xlsx";

            return workbook.Deliver(nombreArchivoExcel,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            
        }

        public async Task<ActionResult> Descargar(int id)
        {

            if(!ModelState.IsValid)
                return BadRequest(ModelState);


            var factura = await _db.Facturas
                .Include(m => m.Lineas)
                .FirstAsync(m => m.Id == id);

            var qr = _verifactu.ObtenerQr(factura);

            if(qr.IsFailure)
                return BadRequest(qr.Error);

            var informeLocal = GeneraLocalReportFactura.GenerarInformeLocalFactura(factura, qr.Value, _env.WebRootPath);
            
            byte[] pdf = informeLocal.Render("PDF");
            var nombrePdf = factura.NumeroYEmpresaFactura().Replace(":", " ").Replace("·", "").Replace("€", "").Replace("/", "-").EliminarDiacriticos() + ".pdf";
            
            var cabecera = new System.Net.Mime.ContentDisposition
            {
                FileName = nombrePdf,

                // Si es verdadero el navegador trata de mostrar el archivo directamente
                Inline = false,
            };

            HttpContext.Response.Headers.Append("Content-Disposition", cabecera.ToString());

            return File(pdf, "application/pdf");
            
        }
    }
}
