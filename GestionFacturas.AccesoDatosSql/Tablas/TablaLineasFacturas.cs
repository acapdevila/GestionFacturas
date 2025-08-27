using GestionFacturas.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionFacturas.AccesoDatosSql.Tablas;

public class TablaLineasFacturas : IEntityTypeConfiguration<LineaFactura>
{
    public void Configure(EntityTypeBuilder<LineaFactura> entity)
    {
        entity.ToTable("FacturasLineas",SqlDb.EsquemaTablas);

        entity.HasIndex(e => e.IdFactura, "IX_IdFactura");

        entity.Property(e => e.Cantidad).HasColumnType("decimal(18, 2)");

        entity.Property(e => e.Descripcion).HasMaxLength(250);

        entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18, 2)");

        entity.HasOne(d => d.Factura)
            .WithMany(p => p.Lineas)
            .HasForeignKey(d => d.IdFactura)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_FacturasLineas_Facturas");


    }
}