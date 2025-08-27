using CSharpFunctionalExtensions;
using GestionFacturas.Dominio;
using Microsoft.EntityFrameworkCore;

namespace GestionFacturas.AccesoDatosSql.Repos
{
    public class CambiarEstadoFacturaRepo //: IRepo<CambiarEstadoFactura.Factura>
    {
        private readonly SqlDb _db;

        public CambiarEstadoFacturaRepo(SqlDb db)
        {
            _db = db;
        }

        public async Task<Maybe<Factura>> GetById(int id)
        {
            var factura = await _db.Facturas.FirstOrDefaultAsync(m => m.Id == id);
            
            if (factura is null) return Maybe.None;
            
            return factura;

        }

        public async Task Update(Factura entity)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE GestionFacturas.Facturas SET EstadoFactura={entity.EstadoFactura} WHERE Id={entity.Id}");
            
      
        }
    }
}
