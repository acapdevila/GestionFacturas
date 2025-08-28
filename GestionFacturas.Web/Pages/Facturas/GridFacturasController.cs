using ClosedXML.Excel;
using ClosedXML.Extensions;
using GestionFacturas.AccesoDatosSql;
using GestionFacturas.Aplicacion;
using GestionFacturas.Dominio;
using GestionFacturas.Dominio.Infra;
using GestionFacturas.Web.Framework.Grid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionFacturas.Web.Pages.Facturas
{
    public class GridFacturasController : Controller
    {
        private readonly SqlDb _db;

        public GridFacturasController(SqlDb context)
        {
            _db = context;
        }

        public async Task<ActionResult<GridSource>> Buscar(
            [FromQuery] GridParamsFacturas gridParams)
        {
            var consulta = _db.Facturas
                .AsNoTracking()
                .FiltrarPorParametros(gridParams)
                .Select(m => new LineaListaGestionFacturas
                {
                    Id = m.Id,
                    IdUsuario = m.IdUsuario,
                    IdComprador = m.IdComprador,
                    FormatoNumeroFactura = m.FormatoNumeroFactura,
                    DescripcionPrimeraLinea = m.DescripcionPrimeraLinea,
                    NumeracionFactura = m.NumeracionFactura,
                    SerieFactura = m.SerieFactura,
                    FechaEmisionFacturaDateTime = m.FechaEmisionFactura,
                    FechaVencimientoFactura = m.FechaVencimientoFactura,
                    EstadoFactura = m.EstadoFactura,
                    BaseImponible = m.Lineas.Sum(l => (decimal?)(l.PrecioUnitario * l.Cantidad)) ?? 0,
                    Impuestos = m.Lineas.Sum(l => (decimal?)Math.Round((l.PrecioUnitario * l.Cantidad * l.PorcentajeImpuesto / 100), 2)) ?? 0,
                    CompradorNombreOEmpresa = m.CompradorNombreOEmpresa,
                    CompradorNombreComercial = m.Comprador.NombreComercial
                })
                .OrderBy_OrdenarPor(gridParams.Orden);


            var source = await consulta.ToGrid(gridParams);

            return source;
        }


        public async Task<ActionResult<GridSource>> ExportarExcel(
            [FromQuery] GridParamsFacturas gridParams)
        {
            var clientes = await _db.Facturas
                .AsNoTracking()
                .FiltrarPorParametros(gridParams)
                .Select(m => new
                {
                    Id = m.Id,
                    //NombreOEmpresa = m.NombreOEmpresa,
                    //Nif = m.NumeroIdentificacionFiscal,
                    //Email = m.Email,
                    //NumFacturas = m.Facturas.Count
                }).ToListAsync(); 

            using var wb = new XLWorkbook();
            var worksheet = wb.Worksheets.Add("Facturas");
            worksheet.Cell(1, 1).InsertTable(clientes);
            worksheet.Columns().AdjustToContents();
            return wb.Deliver(@"Bahia_Facturas_" + DateTime.Now.ToString("yyyy_MM_dd") + ".xlsx");
        }
    }

    public class GridParamsFacturas : GridParams
    {
        public string? NombreOEmpresa { get; set; }

        public DateTime? DesdeFecha => Desde?.FromInputToDateTime();
        public string? Desde { get; set; }


        public DateTime? HastaFecha => Hasta?.FromInputToDateTime();
        public string? Hasta { get; set; } = string.Empty;
        
        public OrdenFacturas Orden { get; set; }

        public EstadoFacturaEnum? EstadoFactura { get; set; }

    }

    public static class GridParamsCarteraClientesExtensiones
    {
        public static IQueryable<Factura> FiltrarPorParametros(
            this IQueryable<Factura> consulta,
            GridParamsFacturas gridParams)
        {
            return consulta
                .If(!string.IsNullOrEmpty(gridParams.NombreOEmpresa))
                    .ThenWhere(m => m.CompradorNombreOEmpresa.Contains(gridParams.NombreOEmpresa!))
                    
                .If(!string.IsNullOrEmpty(gridParams.Desde))
                    .ThenWhere(m => gridParams.DesdeFecha! <= m.FechaEmisionFactura)
                .If(!string.IsNullOrEmpty(gridParams.Hasta))
                    .ThenWhere(m => m.FechaEmisionFactura <= gridParams.HastaFecha!)

                .If(gridParams.EstadoFactura is not null)
                    .ThenWhere(m => gridParams.EstadoFactura! == m.EstadoFactura)

              ;

        }

    }
}
