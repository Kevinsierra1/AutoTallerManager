using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TipoServicioEnMiniOrdenYEmpleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TipoServicioId",
                table: "MiniOrdenes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TipoServicioId",
                table: "Empleados",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenes_TipoServicioId",
                table: "MiniOrdenes",
                column: "TipoServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_TipoServicioId",
                table: "Empleados",
                column: "TipoServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_TiposServicio_TipoServicioId",
                table: "Empleados",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MiniOrdenes_TiposServicio_TipoServicioId",
                table: "MiniOrdenes",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_TiposServicio_TipoServicioId",
                table: "Empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniOrdenes_TiposServicio_TipoServicioId",
                table: "MiniOrdenes");

            migrationBuilder.DropIndex(
                name: "IX_MiniOrdenes_TipoServicioId",
                table: "MiniOrdenes");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_TipoServicioId",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "TipoServicioId",
                table: "MiniOrdenes");

            migrationBuilder.DropColumn(
                name: "TipoServicioId",
                table: "Empleados");
        }
    }
}
