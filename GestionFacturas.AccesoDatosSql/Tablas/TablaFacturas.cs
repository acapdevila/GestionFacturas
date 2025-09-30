using GestionFacturas.Dominio;
using GestionFacturas.Dominio.Clientes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionFacturas.AccesoDatosSql.Tablas
{
    public class TablaFacturas : IEntityTypeConfiguration<Factura>
    {
        public void Configure(EntityTypeBuilder<Factura> entity)
        {
            entity.ToTable("Facturas", SqlDb.EsquemaTablas);

            entity.HasIndex(e => e.IdComprador, "IX_IdComprador");

            entity.HasIndex(e => e.IdUsuario, "IX_IdUsuario");

            entity.Property(e => e.ComentarioInterno).HasMaxLength(250).IsRequired(false);

            entity.Property(e => e.DescripcionPrimeraLinea).HasMaxLength(250).IsRequired();

            entity.Property(e => e.Comentarios).HasMaxLength(250).IsRequired(false);

            entity.Property(e => e.ComentariosPie).HasMaxLength(800).IsRequired(false);

            entity.Property(e => e.CompradorCodigoPostal).HasMaxLength(10);

            entity.Property(e => e.CompradorDireccion).HasMaxLength(128);

            entity.Property(e => e.CompradorEmail).HasMaxLength(256);

            entity.Property(e => e.CompradorLocalidad).HasMaxLength(50);

            entity.Property(e => e.CompradorNombreOEmpresa)
                .HasMaxLength(128)
                .HasColumnName("CompradorNombreOEmpresa");

            entity.Property(e => e.CompradorNumeroIdentificacionFiscal).HasMaxLength(50);

            entity.Property(e => e.CompradorCodigoPaisIso2).HasMaxLength(32);

            entity.Property(e => e.CompradorProvincia).HasMaxLength(50);

            entity.Property(e => e.FechaEmisionFactura).HasColumnType("datetime");

            entity.Property(e => e.FechaVencimientoFactura).HasColumnType("datetime");

            entity.Property(e => e.FormaPagoDetalles).HasMaxLength(50);

            entity.Property(e => e.FormatoNumeroFactura).HasMaxLength(50);

            entity.Property(e => e.IdUsuario).HasMaxLength(128);

            entity.Property(e => e.NombreArchivoPlantillaInforme).HasMaxLength(50);

            entity.Property(e => e.SerieFactura).HasMaxLength(50);

            entity.Property(e => e.VendedorCodigoPostal).HasMaxLength(10);

            entity.Property(e => e.VendedorDireccion).HasMaxLength(128);

            entity.Property(e => e.VendedorEmail).HasMaxLength(256);

            entity.Property(e => e.VendedorLocalidad).HasMaxLength(50);

            entity.Property(e => e.VendedorNombreOEmpresa)
                .HasMaxLength(50)
                .HasColumnName("VendedorNombreOEmpresa");

            entity.Property(e => e.VendedorNumeroIdentificacionFiscal).HasMaxLength(50);

            entity.Property(e => e.VendedorProvincia).HasMaxLength(50);

            entity.Property(e => e.VerifactuCsv).HasMaxLength(128);

            entity.Property(e => e.EnviadaAHacienda).HasDefaultValue(false);

            entity.HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(u => u.IdUsuario)
                .IsRequired();

            entity.HasOne(d => d.Comprador)
                .WithMany(p => p.Facturas)
                .HasForeignKey(d => d.IdComprador)
                .HasConstraintName("FK_dbo.Facturas_dbo.Clientes_IdComprador");

        }
    }
}
