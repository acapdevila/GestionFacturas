using GestionFacturas.AccesoDatosSql;
using Microsoft.EntityFrameworkCore.Design;

namespace GestionFacturas.Web.Framework
{
    public class DbContextFactory : IDesignTimeDbContextFactory<SqlDb>
    {
        public SqlDb CreateDbContext(string[] args)
        {
           var  configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", true)
                .Build();


            return new SqlDb(configuration.GetConnectionString("ConnectionString")!);
        }
    }
}