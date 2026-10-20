using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_copia_EB_JZ.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarEstructuraPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "anillado",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "color",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "copias",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "doble_faz",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "tamano_hoja",
                table: "DetallesPedido");

            migrationBuilder.AddColumn<string>(
                name: "Anillado",
                table: "Documentos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Color",
                table: "Documentos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Copias",
                table: "Documentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DobleFaz",
                table: "Documentos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Precio",
                table: "Documentos",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "TamanoHoja",
                table: "Documentos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anillado",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Copias",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "DobleFaz",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "TamanoHoja",
                table: "Documentos");

            migrationBuilder.AddColumn<string>(
                name: "anillado",
                table: "DetallesPedido",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "color",
                table: "DetallesPedido",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "copias",
                table: "DetallesPedido",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "doble_faz",
                table: "DetallesPedido",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "tamano_hoja",
                table: "DetallesPedido",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
