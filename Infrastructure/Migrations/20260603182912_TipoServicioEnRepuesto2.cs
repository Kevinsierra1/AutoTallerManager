using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TipoServicioEnRepuesto2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TipoServicioId",
                table: "Repuestos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repuestos_TipoServicioId",
                table: "Repuestos",
                column: "TipoServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repuestos_TiposServicio_TipoServicioId",
                table: "Repuestos",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repuestos_TiposServicio_TipoServicioId",
                table: "Repuestos");

            migrationBuilder.DropIndex(
                name: "IX_Repuestos_TipoServicioId",
                table: "Repuestos");

            migrationBuilder.DropColumn(
                name: "TipoServicioId",
                table: "Repuestos");
        }
    }
}
