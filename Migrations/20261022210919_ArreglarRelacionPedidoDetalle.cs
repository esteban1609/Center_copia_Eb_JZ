using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_copia_EB_JZ.Migrations
{
    /// <inheritdoc />
    public partial class ArreglarRelacionPedidoDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Pedidos_Pedidoid_pedido",
                table: "DetallesPedido");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPedido_Pedidoid_pedido",
                table: "DetallesPedido");

            migrationBuilder.DropColumn(
                name: "Pedidoid_pedido",
                table: "DetallesPedido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pedidoid_pedido",
                table: "DetallesPedido",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_Pedidoid_pedido",
                table: "DetallesPedido",
                column: "Pedidoid_pedido");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPedido_Pedidos_Pedidoid_pedido",
                table: "DetallesPedido",
                column: "Pedidoid_pedido",
                principalTable: "Pedidos",
                principalColumn: "id_pedido");
        }
    }
}
