using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionFacturas.AccesoDatosSql.Migrations
{
    public partial class Migracion_Cliente_y_Factura_CodigoPaisIso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompradorCodigoPaisIso2",
                schema: "GestionFacturas",
                table: "Facturas",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoPaisIso2",
                schema: "GestionFacturas",
                table: "Clientes",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompradorCodigoPaisIso2",
                schema: "GestionFacturas",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "CodigoPaisIso2",
                schema: "GestionFacturas",
                table: "Clientes");
        }
    }
}
