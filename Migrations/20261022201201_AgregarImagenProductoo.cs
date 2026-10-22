using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_copia_EB_JZ.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenProductoo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RutaImagen",
                table: "Productos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RutaImagen",
                table: "Productos");
        }
    }
}
