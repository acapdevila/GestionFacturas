using CSharpFunctionalExtensions;
using GestionFacturas.Dominio.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace GestionFacturas.AccesoDatosSql
{
    public static class DbInitializer
    {
        public static void EjecutarMigracionesPendientesYSincronizarTablas(SqlDb db)
        {
            var migracionesPendientes = db.Database.GetPendingMigrations();
            
            if(!migracionesPendientes.Any()) return;

            db.Database.Migrate();
            
            SincronizarTablas(db);
        }

        public static void EjecutarMigracionesPendientes(SqlDb db)
        {
            var migracionesPendientes = db.Database.GetPendingMigrations();
            if (!migracionesPendientes.Any()) return;

            db.Database.Migrate();
            
        }

        public static void SincronizarTablas(SqlDb db)
        {
            db.SaveChanges();
        }
        

        private static void SincronizarEntidades<T>(SqlDb context, IEnumerable<T> entidades, string nombreTabla) where T : Entity<int>
        {
            using var transaction = context.Database.BeginTransaction();

            context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {SqlDb.Esquema}.{nombreTabla} ON");
            
            foreach (var entidad in entidades)
            {
                SincronizarEntidad(context, entidad);
            }

            context.SaveChanges();
            context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {SqlDb.Esquema}.{nombreTabla} OFF");
            transaction.Commit();
          
        }

        private static void SincronizarEntidad<T>(SqlDb context, T entidad) where T : Entity<int>
        {
            if (context.Set<T>().Any(m => m.Id == entidad.Id))
                context.Set<T>().Update(entidad);
            else
                context.Set<T>().Add(entidad);
        }

       
    }
}

