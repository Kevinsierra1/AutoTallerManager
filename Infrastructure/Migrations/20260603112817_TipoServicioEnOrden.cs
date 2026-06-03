using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TipoServicioEnOrden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TipoServicioId",
                table: "OrdenesServicio",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_TipoServicioId",
                table: "OrdenesServicio",
                column: "TipoServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_TiposServicio_TipoServicioId",
                table: "OrdenesServicio",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_TiposServicio_TipoServicioId",
                table: "OrdenesServicio");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesServicio_TipoServicioId",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "TipoServicioId",
                table: "OrdenesServicio");
        }
    }
}
