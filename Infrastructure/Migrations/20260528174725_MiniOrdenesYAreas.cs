using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MiniOrdenesYAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockCritico",
                table: "Repuestos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Ubicacion",
                table: "Repuestos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AreasTaller",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreasTaller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransferenciasInventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroTransferencia = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Origen = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Destino = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    SolicitadoPorId = table.Column<Guid>(type: "uuid", nullable: true),
                    AprobadoPorId = table.Column<Guid>(type: "uuid", nullable: true),
                    AprobadoPorNombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCompletado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    MotivoRechazo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferenciasInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferenciasInventario_Usuarios_SolicitadoPorId",
                        column: x => x.SolicitadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrdenAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdenServicioId = table.Column<Guid>(type: "uuid", nullable: false),
                    AreaTallerId = table.Column<Guid>(type: "uuid", nullable: false),
                    MecanicoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TotalMateriales = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalManoObra = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenAreas_AreasTaller_AreaTallerId",
                        column: x => x.AreaTallerId,
                        principalTable: "AreasTaller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenAreas_Empleados_MecanicoId",
                        column: x => x.MecanicoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrdenAreas_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferenciaInventarioDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferenciaInventarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepuestoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferenciaInventarioDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferenciaInventarioDetalles_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferenciaInventarioDetalles_TransferenciasInventario_Tr~",
                        column: x => x.TransferenciaInventarioId,
                        principalTable: "TransferenciasInventario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiniOrdenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroMiniOrden = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OrdenServicioId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdenAreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    MecanicoId = table.Column<Guid>(type: "uuid", nullable: true),
                    JefeTallerId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaAprobacionJefe = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaAprobacionCliente = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalMateriales = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalManoObra = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    MotivoRechazo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniOrdenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniOrdenes_Empleados_JefeTallerId",
                        column: x => x.JefeTallerId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MiniOrdenes_Empleados_MecanicoId",
                        column: x => x.MecanicoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MiniOrdenes_OrdenAreas_OrdenAreaId",
                        column: x => x.OrdenAreaId,
                        principalTable: "OrdenAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MiniOrdenes_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenAreaDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdenAreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepuestoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenAreaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenAreaDetalles_OrdenAreas_OrdenAreaId",
                        column: x => x.OrdenAreaId,
                        principalTable: "OrdenAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenAreaDetalles_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenAreaManosObra",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdenAreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TecnicoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    HorasTrabajo = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TarifaHora = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenAreaManosObra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenAreaManosObra_Empleados_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrdenAreaManosObra_OrdenAreas_OrdenAreaId",
                        column: x => x.OrdenAreaId,
                        principalTable: "OrdenAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiniOrdenAprobaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MiniOrdenId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nivel = table.Column<int>(type: "integer", nullable: false),
                    Aprobado = table.Column<bool>(type: "boolean", nullable: false),
                    AprobadoPorId = table.Column<Guid>(type: "uuid", nullable: true),
                    AprobadoPorNombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Observacion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniOrdenAprobaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniOrdenAprobaciones_MiniOrdenes_MiniOrdenId",
                        column: x => x.MiniOrdenId,
                        principalTable: "MiniOrdenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiniOrdenDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MiniOrdenId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepuestoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniOrdenDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniOrdenDetalles_MiniOrdenes_MiniOrdenId",
                        column: x => x.MiniOrdenId,
                        principalTable: "MiniOrdenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MiniOrdenDetalles_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MiniOrdenHistoriales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MiniOrdenId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstadoAnterior = table.Column<int>(type: "integer", nullable: false),
                    EstadoNuevo = table.Column<int>(type: "integer", nullable: false),
                    Observacion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CambiadoPor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NivelAprobacion = table.Column<int>(type: "integer", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniOrdenHistoriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniOrdenHistoriales_MiniOrdenes_MiniOrdenId",
                        column: x => x.MiniOrdenId,
                        principalTable: "MiniOrdenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiniOrdenManosObra",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MiniOrdenId = table.Column<Guid>(type: "uuid", nullable: false),
                    TecnicoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    HorasTrabajo = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TarifaHora = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiniOrdenManosObra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiniOrdenManosObra_Empleados_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MiniOrdenManosObra_MiniOrdenes_MiniOrdenId",
                        column: x => x.MiniOrdenId,
                        principalTable: "MiniOrdenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesInventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroSolicitud = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SolicitanteId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrdenServicioId = table.Column<Guid>(type: "uuid", nullable: true),
                    MiniOrdenId = table.Column<Guid>(type: "uuid", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    AprobadoPorId = table.Column<Guid>(type: "uuid", nullable: true),
                    AprobadoPorNombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaAtencion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    MotivoRechazo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesInventario_MiniOrdenes_MiniOrdenId",
                        column: x => x.MiniOrdenId,
                        principalTable: "MiniOrdenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SolicitudesInventario_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SolicitudesInventario_Usuarios_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudInventarioDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolicitudInventarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepuestoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CantidadSolicitada = table.Column<int>(type: "integer", nullable: false),
                    CantidadAprobada = table.Column<int>(type: "integer", nullable: true),
                    CantidadEntregada = table.Column<int>(type: "integer", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudInventarioDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudInventarioDetalles_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudInventarioDetalles_SolicitudesInventario_Solicitud~",
                        column: x => x.SolicitudInventarioId,
                        principalTable: "SolicitudesInventario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreasTaller_Tipo",
                table: "AreasTaller",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenAprobaciones_MiniOrdenId",
                table: "MiniOrdenAprobaciones",
                column: "MiniOrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenDetalles_MiniOrdenId",
                table: "MiniOrdenDetalles",
                column: "MiniOrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenDetalles_RepuestoId",
                table: "MiniOrdenDetalles",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenes_JefeTallerId",
                table: "MiniOrdenes",
                column: "JefeTallerId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenes_MecanicoId",
                table: "MiniOrdenes",
                column: "MecanicoId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenes_NumeroMiniOrden",
                table: "MiniOrdenes",
                column: "NumeroMiniOrden",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenes_OrdenAreaId",
                table: "MiniOrdenes",
                column: "OrdenAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenes_OrdenServicioId",
                table: "MiniOrdenes",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenHistoriales_MiniOrdenId",
                table: "MiniOrdenHistoriales",
                column: "MiniOrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenManosObra_MiniOrdenId",
                table: "MiniOrdenManosObra",
                column: "MiniOrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_MiniOrdenManosObra_TecnicoId",
                table: "MiniOrdenManosObra",
                column: "TecnicoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAreaDetalles_OrdenAreaId",
                table: "OrdenAreaDetalles",
                column: "OrdenAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAreaDetalles_RepuestoId",
                table: "OrdenAreaDetalles",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAreaManosObra_OrdenAreaId",
                table: "OrdenAreaManosObra",
                column: "OrdenAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAreaManosObra_TecnicoId",
                table: "OrdenAreaManosObra",
                column: "TecnicoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAreas_AreaTallerId",
                table: "OrdenAreas",
                column: "AreaTallerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAreas_MecanicoId",
                table: "OrdenAreas",
                column: "MecanicoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenAreas_OrdenServicioId",
                table: "OrdenAreas",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInventario_MiniOrdenId",
                table: "SolicitudesInventario",
                column: "MiniOrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInventario_NumeroSolicitud",
                table: "SolicitudesInventario",
                column: "NumeroSolicitud",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInventario_OrdenServicioId",
                table: "SolicitudesInventario",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesInventario_SolicitanteId",
                table: "SolicitudesInventario",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudInventarioDetalles_RepuestoId",
                table: "SolicitudInventarioDetalles",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudInventarioDetalles_SolicitudInventarioId",
                table: "SolicitudInventarioDetalles",
                column: "SolicitudInventarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaInventarioDetalles_RepuestoId",
                table: "TransferenciaInventarioDetalles",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaInventarioDetalles_TransferenciaInventarioId",
                table: "TransferenciaInventarioDetalles",
                column: "TransferenciaInventarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciasInventario_NumeroTransferencia",
                table: "TransferenciasInventario",
                column: "NumeroTransferencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciasInventario_SolicitadoPorId",
                table: "TransferenciasInventario",
                column: "SolicitadoPorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MiniOrdenAprobaciones");

            migrationBuilder.DropTable(
                name: "MiniOrdenDetalles");

            migrationBuilder.DropTable(
                name: "MiniOrdenHistoriales");

            migrationBuilder.DropTable(
                name: "MiniOrdenManosObra");

            migrationBuilder.DropTable(
                name: "OrdenAreaDetalles");

            migrationBuilder.DropTable(
                name: "OrdenAreaManosObra");

            migrationBuilder.DropTable(
                name: "SolicitudInventarioDetalles");

            migrationBuilder.DropTable(
                name: "TransferenciaInventarioDetalles");

            migrationBuilder.DropTable(
                name: "SolicitudesInventario");

            migrationBuilder.DropTable(
                name: "TransferenciasInventario");

            migrationBuilder.DropTable(
                name: "MiniOrdenes");

            migrationBuilder.DropTable(
                name: "OrdenAreas");

            migrationBuilder.DropTable(
                name: "AreasTaller");

            migrationBuilder.DropColumn(
                name: "StockCritico",
                table: "Repuestos");

            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "Repuestos");
        }
    }
}
