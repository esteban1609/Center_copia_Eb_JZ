using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_copia_EB_JZ.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentoIdDocumento",
                table: "DetallesPedido",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pedidoid_pedido",
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
                name: "IX_DetallesPedido_Pedidoid_pedido",
                table: "DetallesPedido",
                column: "Pedidoid_pedido");

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
                name: "FK_DetallesPedido_Pedidos_Pedidoid_pedido",
                table: "DetallesPedido",
                column: "Pedidoid_pedido",
                principalTable: "Pedidos",
                principalColumn: "id_pedido");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPedido_Productos_ProductoIdProducto",
                table: "DetallesPedido",
                column: "ProductoIdProducto",
                principalTable: "Productos",
                principalColumn: "IdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Documentos_DocumentoIdDocumento",
                table: "DetallesPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Pedidos_Pedidoid_pedido",
                table: "DetallesPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Productos_ProductoIdProducto",
                table: "DetallesPedido");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPedido_DocumentoIdDocumento",
                table: "DetallesPedido");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPedido_Pedidoid_pedido",
                table: "DetallesPedido");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPedido_ProductoIdProducto",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "activo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DocumentoIdDocumento",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "Pedidoid_pedido",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "ProductoIdProducto",
                table: "DetallesPedido");
        }
    }
}
