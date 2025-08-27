using CSharpFunctionalExtensions;
using GestionFacturas.AccesoDatosSql;
using GestionFacturas.Dominio;
using Microsoft.EntityFrameworkCore;
using VeriFactu.Business;
using VeriFactu.Config;
using VeriFactu.Net.Rest;
using VeriFactu.Xml.Factu.Alta;

namespace GestionFacturas.Aplicacion;

public interface IVerifactuServicio

{
    Task<Result> EnviarAltaFacturaAsync(int facturaId);
    Task<Result> EnviarAnulacionFacturaAsync(int facturaId);

    Task<Result<MemoryStream>> ObtenerQr(int facturaId);
}


public class VerifactuIreneSolutionsServicio : IVerifactuServicio
{
    private readonly SqlDb _db;

    public VerifactuIreneSolutionsServicio(SqlDb db)
    {
        _db = db;
    }

    public async Task<Result> EnviarAltaFacturaAsync(int facturaId)
    {
        GuardarApiKey();
        
        var factura = await BuscarFactura(facturaId);

        if (factura is null)
            return Result.Failure($"Factura no encontrada. Id: {factura}");

        var invoice = ToIreneSolutionsInvoice(factura); 

        dynamic result = ApiClient.Create(invoice);

        if (result.ResultCode != 0)
        {
            return Result.Failure($"Se ha producido un error al llamar al API: {result.ResultMessage}");
        }
        
        var csv = $"{result.Return.CSV}";

        if (string.IsNullOrEmpty(csv))
            return Result.Failure($"El envío no se ha realizado con éxito: {result.Return.ErrorDescription}");

        // Guardar el CSV en la base de datos
        // factura.CodigoSeguridad = csv;

        await _db.SaveChangesAsync();

        return Result.Success();

    }

    private static void  GuardarApiKey()
    {
        Settings.Current.Api.ServiceKey = "My_ServiceKey";
        Settings.Save();
    }

    public async Task<Result<MemoryStream>> ObtenerQr(int facturaId)
    {
        GuardarApiKey();

        var factura = await BuscarFactura(facturaId);

        if (factura is null)
            return Result.Failure<MemoryStream>($"Factura no encontrada. Id: {factura}");

        var invoice = ToIreneSolutionsInvoice(factura);

        dynamic result = ApiClient.GetQr(invoice);

        if (result.ResultCode != 0)
        {
            return Result.Failure<MemoryStream>($"Se ha producido un error al llamar al API: {result.ResultMessage}");
        }

        var bitMapBytes = Convert.FromBase64String(result.Return);
        return new MemoryStream(bitMapBytes);
    }


    public async Task<Result> EnviarAnulacionFacturaAsync(int facturaId)
    {
        GuardarApiKey();

        var factura = await BuscarFactura(facturaId);

        if (factura is null)
            return Result.Failure($"Factura no encontrada. Id: {factura}");

        var invoice = ToIreneSolutionsInvoice(factura);

        dynamic result = ApiClient.Delete(invoice);

        if (result.ResultCode != 0)
        {
            return Result.Failure($"Se ha producido un error al llamar al API: {result.ResultMessage}");
        }

        var csv = $"{result.Return.CSV}";

        if (string.IsNullOrEmpty(csv))
            return Result.Failure($"El envío de anulación no se ha realizado con éxito: {result.Return.ErrorDescription}");

        // Guardar el CSV en la base de datos
        // factura.CodigoSeguridad = csv;


        await _db.SaveChangesAsync();

        return Result.Success();
    }


    private async Task<Factura?> BuscarFactura(int facturaId)
    {
        return await _db.Facturas
            .Include(f => f.Lineas)
            .FirstOrDefaultAsync(f => f.Id == facturaId);
    }

   
   



    private static Invoice ToIreneSolutionsInvoice(Factura factura)
    {
        return new Invoice(factura.NumeroFactura,
            factura.FechaEmisionFactura,
            factura.VendedorNumeroIdentificacionFiscal)
        {
            InvoiceType = TipoFactura.F1,
            SellerName = factura.VendedorNombreOEmpresa,
            BuyerID = factura.CompradorNumeroIdentificacionFiscal,
            BuyerName = factura.CompradorNombreOEmpresa,
            Text = factura.DescripcionPrimeraLinea,
            TaxItems = factura.Lineas.Select(linea => new TaxItem()
            {
                TaxScheme = ClaveRegimen.RegimenGeneral,
                TaxType = CalificacionOperacion.S1,
                TaxRate = linea.PorcentajeImpuesto,
                TaxBase = linea.PrecioXCantidad,
                TaxAmount = linea.ImporteImpuesto
            }).ToList()
        };
    }
}
