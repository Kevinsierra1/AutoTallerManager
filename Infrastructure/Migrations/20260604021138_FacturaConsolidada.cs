using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FacturaConsolidada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_OrdenesServicio_OrdenServicioId",
                table: "Facturas");

            migrationBuilder.AddColumn<Guid>(
                name: "FacturaId",
                table: "OrdenesServicio",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenServicioId",
                table: "Facturas",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_FacturaId",
                table: "OrdenesServicio",
                column: "FacturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_OrdenesServicio_OrdenServicioId",
                table: "Facturas",
                column: "OrdenServicioId",
                principalTable: "OrdenesServicio",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_Facturas_FacturaId",
                table: "OrdenesServicio",
                column: "FacturaId",
                principalTable: "Facturas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_OrdenesServicio_OrdenServicioId",
                table: "Facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_Facturas_FacturaId",
                table: "OrdenesServicio");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesServicio_FacturaId",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "FacturaId",
                table: "OrdenesServicio");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenServicioId",
                table: "Facturas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_OrdenesServicio_OrdenServicioId",
                table: "Facturas",
                column: "OrdenServicioId",
                principalTable: "OrdenesServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
