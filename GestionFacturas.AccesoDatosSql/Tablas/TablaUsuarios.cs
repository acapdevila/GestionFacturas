using GestionFacturas.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionFacturas.AccesoDatosSql.Tablas
{
    public class TablaUsuarios : IEntityTypeConfiguration<Usuario>
    {
      

        public void Configure(EntityTypeBuilder<Usuario> entity)
        {
            entity.ToTable("AspNetUsers",SqlDb.EsquemaTablas);
            entity.Property(e => e.Email);

            entity.Property(e => e.Password).HasColumnName("PasswordHash");
        }
    }
}