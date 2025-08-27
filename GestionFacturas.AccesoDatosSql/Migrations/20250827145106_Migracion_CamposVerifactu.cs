using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionFacturas.AccesoDatosSql.Migrations
{
    public partial class Migracion_CamposVerifactu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnviadaAHacienda",
                schema: "GestionFacturas",
                table: "Facturas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerifactuCsv",
                schema: "GestionFacturas",
                table: "Facturas",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnviadaAHacienda",
                schema: "GestionFacturas",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "VerifactuCsv",
                schema: "GestionFacturas",
                table: "Facturas");
        }
    }
}
