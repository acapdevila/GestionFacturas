using GestionFacturas.Dominio;
using GestionFacturas.Dominio.Clientes;
using Microsoft.EntityFrameworkCore;

namespace GestionFacturas.AccesoDatosSql
{
    public partial class SqlDb : DbContext
    {
        private readonly string _connectionString;

        public const string EsquemaTablas = "GestionFacturas";

        //public SqlDb()
        //{
        //    _connectionString =
        //        @"Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=C:\Proyectos\GestionFacturas\GestionFacturas.Datos\BaseDatosLocal\aspnet-GestionFacturas.Website-20150802075020.mdf;Integrated Security=True";

        //}

        public SqlDb(string connectionString) 
        {
            _connectionString = connectionString;
        }

        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        public virtual DbSet<Cliente> Clientes { get; set; } = null!;
        public virtual DbSet<Factura> Facturas { get; set; } = null!;
        public virtual DbSet<LineaFactura> FacturasLineas { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder
                .UseSqlServer(_connectionString,
                    x => x.MigrationsHistoryTable("__EFGestionFacturasMigrationsHistory", EsquemaTablas))
                .UseLazyLoadingProxies();
                    
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SqlDb).Assembly);
            
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public async Task<Factura> ObtenerUlitmaFacturaDeLaSerie(string serie)
        {
            if (string.IsNullOrEmpty(serie))
            {
                var factura = await Facturas.Where(m => m.SerieFactura != null && m.SerieFactura != "")
                    .OrderByDescending(m => m.FechaEmisionFactura)
                    .FirstOrDefaultAsync();

                if (factura == null) return null;

                serie = factura.SerieFactura;
            }

            var consulta = Facturas.Where(m => m.SerieFactura == serie);

            return await consulta
                .OrderByDescending(m => m.NumeracionFactura)
                .FirstAsync();
        }
    }
}
