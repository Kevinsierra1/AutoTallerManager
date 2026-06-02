using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VinculoClienteUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MiniOrdenes_OrdenesServicio_OrdenServicioId",
                table: "MiniOrdenes");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenServicioId",
                table: "MiniOrdenes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ClienteId",
                table: "MiniOrdenes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VehiculoId",
                table: "MiniOrdenes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Clientes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenes_ClienteId",
                table: "MiniOrdenes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenes_VehiculoId",
                table: "MiniOrdenes",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniOrdenes_Clientes_ClienteId",
                table: "MiniOrdenes",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniOrdenes_OrdenesServicio_OrdenServicioId",
                table: "MiniOrdenes",
                column: "OrdenServicioId",
                principalTable: "OrdenesServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniOrdenes_Vehiculos_VehiculoId",
                table: "MiniOrdenes",
                column: "VehiculoId",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniOrdenes_Clientes_ClienteId",
                table: "MiniOrdenes");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniOrdenes_OrdenesServicio_OrdenServicioId",
                table: "MiniOrdenes");

            migrationBuilder.DropForeignKey(
                name: "FK_MiniOrdenes_Vehiculos_VehiculoId",
                table: "MiniOrdenes");

            migrationBuilder.DropIndex(
                name: "IX_MiniOrdenes_ClienteId",
                table: "MiniOrdenes");

            migrationBuilder.DropIndex(
                name: "IX_MiniOrdenes_VehiculoId",
                table: "MiniOrdenes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "MiniOrdenes");

            migrationBuilder.DropColumn(
                name: "VehiculoId",
                table: "MiniOrdenes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Clientes");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrdenServicioId",
                table: "MiniOrdenes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MiniOrdenes_OrdenesServicio_OrdenServicioId",
                table: "MiniOrdenes",
                column: "OrdenServicioId",
                principalTable: "OrdenesServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
