using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_copia_EB_JZ.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPaginasDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Paginas",
                table: "Documentos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paginas",
                table: "Documentos");
        }
    }
}
