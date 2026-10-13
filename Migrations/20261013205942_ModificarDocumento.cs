using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_copia_EB_JZ.Migrations
{
    /// <inheritdoc />
    public partial class ModificarDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdPedido",
                table: "Documentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_IdPedido",
                table: "Documentos",
                column: "IdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_Pedidos_IdPedido",
                table: "Documentos",
                column: "IdPedido",
                principalTable: "Pedidos",
                principalColumn: "id_pedido",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_Pedidos_IdPedido",
                table: "Documentos");

            migrationBuilder.DropIndex(
                name: "IX_Documentos_IdPedido",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "IdPedido",
                table: "Documentos");
        }
    }
}
