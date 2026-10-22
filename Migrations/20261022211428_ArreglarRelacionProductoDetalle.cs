using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_copia_EB_JZ.Migrations
{
    /// <inheritdoc />
    public partial class ArreglarRelacionProductoDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Documentos_DocumentoIdDocumento",
                table: "DetallesPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Productos_ProductoIdProducto",
                table: "DetallesPedido");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPedido_DocumentoIdDocumento",
                table: "DetallesPedido");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPedido_ProductoIdProducto",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "DocumentoIdDocumento",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "ProductoIdProducto",
                table: "DetallesPedido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentoIdDocumento",
                table: "DetallesPedido",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductoIdProducto",
                table: "DetallesPedido",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_DocumentoIdDocumento",
                table: "DetallesPedido",
                column: "DocumentoIdDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_ProductoIdProducto",
                table: "DetallesPedido",
                column: "ProductoIdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPedido_Documentos_DocumentoIdDocumento",
                table: "DetallesPedido",
                column: "DocumentoIdDocumento",
                principalTable: "Documentos",
                principalColumn: "IdDocumento");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPedido_Productos_ProductoIdProducto",
                table: "DetallesPedido",
                column: "ProductoIdProducto",
                principalTable: "Productos",
                principalColumn: "IdProducto");
        }
    }
}
