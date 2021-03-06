﻿using GestionFacturas.Datos.Configuraciones;
using GestionFacturas.Modelos;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace GestionFacturas.Datos
{
 
    public class ContextoBaseDatos : IdentityDbContext<Usuario>
    {
        public ContextoBaseDatos()
            : base("CadenaConexionGestionFacturas", throwIfV1Schema: false)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<LineaFactura> LineasFacturas { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var esquema = "GestionFacturas";

            modelBuilder.HasDefaultSchema(esquema);

            modelBuilder.Configurations.Add(new ConfiguracionTablaUsuarios());

            modelBuilder.Configurations.Add(new ConfiguracionTablaFacturas(esquema));
            modelBuilder.Configurations.Add(new ConfiguracionTablaLineasFacturas(esquema));
            modelBuilder.Configurations.Add(new ConfiguracionTablaClientes(esquema));

            base.OnModelCreating(modelBuilder);
        }

        public static ContextoBaseDatos Create()
        {
            return new ContextoBaseDatos();
        }
        
    }
}
