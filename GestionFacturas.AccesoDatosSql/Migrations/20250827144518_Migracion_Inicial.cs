using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionFacturas.AccesoDatosSql.Migrations
{
    public partial class Migracion_Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "GestionFacturas");

            //migrationBuilder.CreateTable(
            //    name: "AspNetUsers",
            //    schema: "GestionFacturas",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AspNetUsers", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Clientes",
            //    schema: "GestionFacturas",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        NumeroIdentificacionFiscal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        NombreOEmpresa = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        NombreComercial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        Direccion = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
            //        Localidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        Provincia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        CodigoPostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
            //        Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //        PersonaContacto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        ComentarioInterno = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Clientes", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Facturas",
            //    schema: "GestionFacturas",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        IdUsuario = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        SerieFactura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        NumeracionFactura = table.Column<int>(type: "int", nullable: false),
            //        FormatoNumeroFactura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        FechaEmisionFactura = table.Column<DateTime>(type: "datetime", nullable: false),
            //        FechaVencimientoFactura = table.Column<DateTime>(type: "datetime", nullable: true),
            //        FormaPago = table.Column<int>(type: "int", nullable: false),
            //        FormaPagoDetalles = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        IdVendedor = table.Column<int>(type: "int", nullable: true),
            //        VendedorNumeroIdentificacionFiscal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        VendedorNombreOEmpresa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        VendedorDireccion = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        VendedorLocalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        VendedorProvincia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        VendedorCodigoPostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
            //        VendedorEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //        DescripcionPrimeraLinea = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            //        IdComprador = table.Column<int>(type: "int", nullable: true),
            //        CompradorNumeroIdentificacionFiscal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        CompradorNombreOEmpresa = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        CompradorDireccion = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            //        CompradorLocalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        CompradorProvincia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        CompradorCodigoPostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
            //        CompradorEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //        EstadoFactura = table.Column<int>(type: "int", nullable: false),
            //        Comentarios = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
            //        ComentariosPie = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
            //        ComentarioInterno = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
            //        NombreArchivoPlantillaInforme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Facturas", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_dbo.Facturas_dbo.Clientes_IdComprador",
            //            column: x => x.IdComprador,
            //            principalSchema: "GestionFacturas",
            //            principalTable: "Clientes",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_Facturas_AspNetUsers_IdUsuario",
            //            column: x => x.IdUsuario,
            //            principalSchema: "GestionFacturas",
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "FacturasLineas",
            //    schema: "GestionFacturas",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        IdFactura = table.Column<int>(type: "int", nullable: false),
            //        Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            //        Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        PorcentajeImpuesto = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FacturasLineas", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_FacturasLineas_Facturas",
            //            column: x => x.IdFactura,
            //            principalSchema: "GestionFacturas",
            //            principalTable: "Facturas",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_IdComprador",
            //    schema: "GestionFacturas",
            //    table: "Facturas",
            //    column: "IdComprador");

            //migrationBuilder.CreateIndex(
            //    name: "IX_IdUsuario",
            //    schema: "GestionFacturas",
            //    table: "Facturas",
            //    column: "IdUsuario");

            //migrationBuilder.CreateIndex(
            //    name: "IX_IdFactura",
            //    schema: "GestionFacturas",
            //    table: "FacturasLineas",
            //    column: "IdFactura");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "FacturasLineas",
            //    schema: "GestionFacturas");

            //migrationBuilder.DropTable(
            //    name: "Facturas",
            //    schema: "GestionFacturas");

            //migrationBuilder.DropTable(
            //    name: "Clientes",
            //    schema: "GestionFacturas");

            //migrationBuilder.DropTable(
            //    name: "AspNetUsers",
            //    schema: "GestionFacturas");
        }
    }
}
