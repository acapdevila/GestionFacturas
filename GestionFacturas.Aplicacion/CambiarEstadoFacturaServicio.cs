using CSharpFunctionalExtensions;
using GestionFacturas.AccesoDatosSql.Repos;
using GestionFacturas.Dominio;

namespace GestionFacturas.Aplicacion;


public record CambiarEstadoFacturaComando(int Id, EstadoFacturaEnum Estado);

public class CambiarEstadoFacturaServicio
{
    private readonly CambiarEstadoFacturaRepo _db;
        
    public CambiarEstadoFacturaServicio(CambiarEstadoFacturaRepo db)
    {
        _db = db;
    }

    public async Task<Result> Ejecutar(CambiarEstadoFacturaComando comando)
    {
        var posibleFactura = await _db.GetById(comando.Id);
        
        var cambioEstado = posibleFactura.ToResult("Factura no encontrada")
                           .Map(factura =>
                              factura.CambiarEstado(comando.Estado));


        return await cambioEstado.OnSuccessTry(_db.Update, e => e.ToString());
            
    }
}