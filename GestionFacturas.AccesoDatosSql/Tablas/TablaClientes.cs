using CSharpFunctionalExtensions;
using GestionFacturas.Dominio.Clientes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionFacturas.AccesoDatosSql.Tablas
{
    public class TablaClientes : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> entity)
        {
            entity.ToTable("Clientes", SqlDb.EsquemaTablas);

            entity.Property(e => e.CodigoPostal).HasMaxLength(10);

            entity.Property(e => e.ComentarioInterno)
                .IsRequired(false)
                .HasMaxLength(250);

            entity.Property(e => e.Direccion).HasMaxLength(128);

            entity.Property(e => e.Email).HasMaxLength(256);

            entity.Property(e => e.Localidad).HasMaxLength(50);

            entity.Property(e => e.NombreComercial).HasMaxLength(50);

            entity.Property(e => e.NombreOEmpresa)
                .HasMaxLength(128)
                .HasColumnName("NombreOEmpresa");

            entity.Property(e => e.NumeroIdentificacionFiscal).HasMaxLength(50);

            entity.Property(e => e.PersonaContacto).HasMaxLength(50).IsRequired(false);

            entity.Property(e => e.Provincia).HasMaxLength(50);
        }
    }
    

}
