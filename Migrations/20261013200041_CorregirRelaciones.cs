using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_copia_EB_JZ.Migrations
{
    /// <inheritdoc />
    public partial class CorregirRelaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Productos_id_producto",
                table: "DetallesPedido");

            migrationBuilder.AlterColumn<int>(
                name: "id_producto",
                table: "DetallesPedido",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id_documento",
                table: "DetallesPedido",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_id_pedido",
                table: "DetallesPedido",
                column: "id_pedido");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPedido_Pedidos_id_pedido",
                table: "DetallesPedido",
                column: "id_pedido",
                principalTable: "Pedidos",
                principalColumn: "id_pedido",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPedido_Productos_id_producto",
                table: "DetallesPedido",
                column: "id_producto",
                principalTable: "Productos",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Pedidos_id_pedido",
                table: "DetallesPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPedido_Productos_id_producto",
                table: "DetallesPedido");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPedido_id_pedido",
                table: "DetallesPedido");

            migrationBuilder.AlterColumn<int>(
                name: "id_producto",
                table: "DetallesPedido",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "id_documento",
                table: "DetallesPedido",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPedido_Productos_id_producto",
                table: "DetallesPedido",
                column: "id_producto",
                principalTable: "Productos",
                principalColumn: "IdProducto");
        }
    }
}
