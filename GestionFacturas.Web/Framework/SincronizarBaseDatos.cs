using GestionFacturas.AccesoDatosSql;

namespace GestionFacturas.Web.Framework
{
    public static class SincronizarBaseDatosExtension
    {

        public static IHost SincronizarBaseDatos(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<SqlDb>();
            DbInitializer.EjecutarMigracionesPendientesYSincronizarTablas(context);
            DbInitializer.SincronizarTablas(context);

            return host;

        }
    }
}
