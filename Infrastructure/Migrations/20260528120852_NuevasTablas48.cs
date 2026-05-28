using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NuevasTablas48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TipoCombustibleId",
                table: "Vehiculos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TipoTransmisionId",
                table: "Vehiculos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrioridadOrdenId",
                table: "OrdenesServicio",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EstadoFacturaId",
                table: "Facturas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfiguracionesSistema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Clave = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "string"),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Grupo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_ConfiguracionesSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CorreosClientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Principal = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_CorreosClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorreosClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleFacturas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacturaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Impuesto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleFacturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleFacturas_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DireccionesClientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Linea1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Linea2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Ciudad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Departamento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Pais = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "Colombia"),
                    CodigoPostal = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Principal = table.Column<bool>(type: "boolean", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DireccionesClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DireccionesClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntradasInventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RepuestoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProveedorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumeroFacturaProveedor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MovimientoInventarioId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_EntradasInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntradasInventario_MovimientosInventario_MovimientoInventar~",
                        column: x => x.MovimientoInventarioId,
                        principalTable: "MovimientosInventario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntradasInventario_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EntradasInventario_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCita",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_EstadosCita", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosFactura",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_EstadosFactura", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosOrden",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Codigo = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_EstadosOrden", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistorialAccesos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaAcceso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Exitoso = table.Column<bool>(type: "boolean", nullable: false),
                    MotivoFallo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialAccesos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialAccesos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialCitas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CitaId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstadoAnterior = table.Column<int>(type: "integer", nullable: false),
                    EstadoNuevo = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCambio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CambiadoPor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCitas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialCitas_Citas_CitaId",
                        column: x => x.CitaId,
                        principalTable: "Citas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialPreciosRepuestos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RepuestoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrecioCompraAnterior = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecioCompraNuevo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecioVentaAnterior = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecioVentaNuevo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CambiadoPor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialPreciosRepuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialPreciosRepuestos_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImpuestosFacturas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacturaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Porcentaje = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Base = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpuestosFacturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImpuestosFacturas_Facturas_FacturaId",
                        column: x => x.FacturaId,
                        principalTable: "Facturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Mensaje = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Info"),
                    Leida = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaLectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrdenServiciosExtras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdenServicioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Costo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenServiciosExtras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenServiciosExtras_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Clave = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Modulo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrioridadesOrden",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nivel = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_PrioridadesOrden", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalidasInventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RepuestoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FechaSalida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrdenServicioId = table.Column<Guid>(type: "uuid", nullable: true),
                    MovimientoInventarioId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_SalidasInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalidasInventario_MovimientosInventario_MovimientoInventari~",
                        column: x => x.MovimientoInventarioId,
                        principalTable: "MovimientosInventario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SalidasInventario_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SalidasInventario_Repuestos_RepuestoId",
                        column: x => x.RepuestoId,
                        principalTable: "Repuestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SesionesUsuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TokenSesion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionesUsuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SesionesUsuarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelefonosClientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Principal = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_TelefonosClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelefonosClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TiposCombustible",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_TiposCombustible", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposMovimientoInventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_TiposMovimientoInventario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposTransmision",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_TiposTransmision", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehiculoDocumentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehiculoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UrlDocumento = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaSubida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiculoDocumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiculoDocumentos_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehiculoFotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehiculoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UrlFoto = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FechaSubida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Principal = table.Column<bool>(type: "boolean", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiculoFotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiculoFotos_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehiculoKilometrajes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehiculoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kilometraje = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RegistradoPor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiculoKilometrajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiculoKilometrajes_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehiculoMantenimientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehiculoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FechaMantenimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProximoMantenimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KilometrajeMantenimiento = table.Column<int>(type: "integer", nullable: false),
                    ProximoKilometraje = table.Column<int>(type: "integer", nullable: true),
                    OrdenServicioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Costo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    EliminadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiculoMantenimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiculoMantenimientos_OrdenesServicio_OrdenServicioId",
                        column: x => x.OrdenServicioId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehiculoMantenimientos_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolPermisos",
                columns: table => new
                {
                    RolId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermisoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolPermisos", x => new { x.RolId, x.PermisoId });
                    table.ForeignKey(
                        name: "FK_RolPermisos_Permisos_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "Permisos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolPermisos_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ConfiguracionesSistema",
                columns: new[] { "Id", "Activo", "ActualizadoEn", "ActualizadoPor", "Clave", "CreadoEn", "CreadoPor", "Descripcion", "Eliminado", "EliminadoEn", "Grupo", "Tipo", "Valor" },
                values: new object[,]
                {
                    { new Guid("99999999-0001-0001-0001-000000000001"), true, null, null, "app.nombre", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "General", "string", "AutoTallerManager" },
                    { new Guid("99999999-0001-0001-0001-000000000002"), true, null, null, "app.version", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "General", "string", "1.0.0" },
                    { new Guid("99999999-0001-0001-0001-000000000003"), true, null, null, "factura.iva", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Facturación", "int", "19" },
                    { new Guid("99999999-0001-0001-0001-000000000004"), true, null, null, "stock.alertas", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Inventario", "bool", "true" },
                    { new Guid("99999999-0001-0001-0001-000000000005"), true, null, null, "taller.moneda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "General", "string", "COP" }
                });

            migrationBuilder.InsertData(
                table: "EstadosCita",
                columns: new[] { "Id", "Activo", "ActualizadoEn", "ActualizadoPor", "CreadoEn", "CreadoPor", "Descripcion", "Eliminado", "EliminadoEn", "Nombre" },
                values: new object[,]
                {
                    { new Guid("33333333-0001-0001-0001-000000000001"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Pendiente" },
                    { new Guid("33333333-0001-0001-0001-000000000002"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Confirmada" },
                    { new Guid("33333333-0001-0001-0001-000000000003"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Completada" },
                    { new Guid("33333333-0001-0001-0001-000000000004"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Cancelada" }
                });

            migrationBuilder.InsertData(
                table: "EstadosFactura",
                columns: new[] { "Id", "Activo", "ActualizadoEn", "ActualizadoPor", "CreadoEn", "CreadoPor", "Descripcion", "Eliminado", "EliminadoEn", "Nombre" },
                values: new object[,]
                {
                    { new Guid("44444444-0001-0001-0001-000000000001"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Pendiente" },
                    { new Guid("44444444-0001-0001-0001-000000000002"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Parcial" },
                    { new Guid("44444444-0001-0001-0001-000000000003"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Pagada" },
                    { new Guid("44444444-0001-0001-0001-000000000004"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Anulada" },
                    { new Guid("44444444-0001-0001-0001-000000000005"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Vencida" }
                });

            migrationBuilder.InsertData(
                table: "EstadosOrden",
                columns: new[] { "Id", "Activo", "ActualizadoEn", "ActualizadoPor", "Codigo", "CreadoEn", "CreadoPor", "Descripcion", "Eliminado", "EliminadoEn", "Nombre" },
                values: new object[,]
                {
                    { new Guid("11111111-0001-0001-0001-000000000001"), true, null, null, 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Pendiente" },
                    { new Guid("11111111-0001-0001-0001-000000000002"), true, null, null, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Aprobada" },
                    { new Guid("11111111-0001-0001-0001-000000000003"), true, null, null, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "En Proceso" },
                    { new Guid("11111111-0001-0001-0001-000000000004"), true, null, null, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Finalizada" },
                    { new Guid("11111111-0001-0001-0001-000000000005"), true, null, null, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Cancelada" }
                });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Activo", "ActualizadoEn", "ActualizadoPor", "Clave", "CreadoEn", "CreadoPor", "Descripcion", "Eliminado", "EliminadoEn", "Modulo", "Nombre" },
                values: new object[,]
                {
                    { new Guid("88888888-0001-0001-0001-000000000001"), true, null, null, "clientes:ver", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Clientes", "Ver Clientes" },
                    { new Guid("88888888-0001-0001-0001-000000000002"), true, null, null, "clientes:crear", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Clientes", "Crear Clientes" },
                    { new Guid("88888888-0001-0001-0001-000000000003"), true, null, null, "vehiculos:ver", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Vehículos", "Ver Vehículos" },
                    { new Guid("88888888-0001-0001-0001-000000000004"), true, null, null, "ordenes:ver", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Órdenes", "Ver Órdenes" },
                    { new Guid("88888888-0001-0001-0001-000000000005"), true, null, null, "inventario:ver", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Inventario", "Ver Inventario" },
                    { new Guid("88888888-0001-0001-0001-000000000006"), true, null, null, "facturas:ver", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Facturación", "Ver Facturas" },
                    { new Guid("88888888-0001-0001-0001-000000000007"), true, null, null, "sistema:admin", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Sistema", "Administrar Todo" }
                });

            migrationBuilder.InsertData(
                table: "PrioridadesOrden",
                columns: new[] { "Id", "Activo", "ActualizadoEn", "ActualizadoPor", "CreadoEn", "CreadoPor", "Descripcion", "Eliminado", "EliminadoEn", "Nivel", "Nombre" },
                values: new object[,]
                {
                    { new Guid("55555555-0001-0001-0001-000000000001"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, 1, "Alta" },
                    { new Guid("55555555-0001-0001-0001-000000000002"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, 2, "Media" },
                    { new Guid("55555555-0001-0001-0001-000000000003"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, 3, "Baja" }
                });

            migrationBuilder.InsertData(
                table: "TiposCombustible",
                columns: new[] { "Id", "Activo", "ActualizadoEn", "ActualizadoPor", "CreadoEn", "CreadoPor", "Eliminado", "EliminadoEn", "Nombre" },
                values: new object[,]
                {
                    { new Guid("66666666-0001-0001-0001-000000000001"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, "Gasolina" },
                    { new Guid("66666666-0001-0001-0001-000000000002"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, "Diesel" },
                    { new Guid("66666666-0001-0001-0001-000000000003"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, "Híbrido" },
                    { new Guid("66666666-0001-0001-0001-000000000004"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, "Eléctrico" },
                    { new Guid("66666666-0001-0001-0001-000000000005"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, "Gas Natural" }
                });

            migrationBuilder.InsertData(
                table: "TiposMovimientoInventario",
                columns: new[] { "Id", "Activo", "ActualizadoEn", "ActualizadoPor", "CreadoEn", "CreadoPor", "Descripcion", "Eliminado", "EliminadoEn", "Nombre" },
                values: new object[,]
                {
                    { new Guid("22222222-0001-0001-0001-000000000001"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Entrada" },
                    { new Guid("22222222-0001-0001-0001-000000000002"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Salida" },
                    { new Guid("22222222-0001-0001-0001-000000000003"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, null, "Ajuste" }
                });

            migrationBuilder.InsertData(
                table: "TiposTransmision",
                columns: new[] { "Id", "Activo", "ActualizadoEn", "ActualizadoPor", "CreadoEn", "CreadoPor", "Eliminado", "EliminadoEn", "Nombre" },
                values: new object[,]
                {
                    { new Guid("77777777-0001-0001-0001-000000000001"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, "Manual" },
                    { new Guid("77777777-0001-0001-0001-000000000002"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, "Automática" },
                    { new Guid("77777777-0001-0001-0001-000000000003"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, "Semiautomática" },
                    { new Guid("77777777-0001-0001-0001-000000000004"), true, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null, "CVT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_TipoCombustibleId",
                table: "Vehiculos",
                column: "TipoCombustibleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_TipoTransmisionId",
                table: "Vehiculos",
                column: "TipoTransmisionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_PrioridadOrdenId",
                table: "OrdenesServicio",
                column: "PrioridadOrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_EstadoFacturaId",
                table: "Facturas",
                column: "EstadoFacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesSistema_Clave",
                table: "ConfiguracionesSistema",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CorreosClientes_ClienteId",
                table: "CorreosClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleFacturas_FacturaId",
                table: "DetalleFacturas",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_DireccionesClientes_ClienteId",
                table: "DireccionesClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_EntradasInventario_MovimientoInventarioId",
                table: "EntradasInventario",
                column: "MovimientoInventarioId");

            migrationBuilder.CreateIndex(
                name: "IX_EntradasInventario_ProveedorId",
                table: "EntradasInventario",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_EntradasInventario_RepuestoId",
                table: "EntradasInventario",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosOrden_Codigo",
                table: "EstadosOrden",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialAccesos_UsuarioId",
                table: "HistorialAccesos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCitas_CitaId",
                table: "HistorialCitas",
                column: "CitaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialPreciosRepuestos_RepuestoId",
                table: "HistorialPreciosRepuestos",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImpuestosFacturas_FacturaId",
                table: "ImpuestosFacturas",
                column: "FacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenServiciosExtras_OrdenServicioId",
                table: "OrdenServiciosExtras",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Clave",
                table: "Permisos",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrioridadesOrden_Nivel",
                table: "PrioridadesOrden",
                column: "Nivel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_PermisoId",
                table: "RolPermisos",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_SalidasInventario_MovimientoInventarioId",
                table: "SalidasInventario",
                column: "MovimientoInventarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SalidasInventario_OrdenServicioId",
                table: "SalidasInventario",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_SalidasInventario_RepuestoId",
                table: "SalidasInventario",
                column: "RepuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_SesionesUsuarios_UsuarioId",
                table: "SesionesUsuarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TelefonosClientes_ClienteId",
                table: "TelefonosClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoDocumentos_VehiculoId",
                table: "VehiculoDocumentos",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoFotos_VehiculoId",
                table: "VehiculoFotos",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoKilometrajes_VehiculoId",
                table: "VehiculoKilometrajes",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoMantenimientos_OrdenServicioId",
                table: "VehiculoMantenimientos",
                column: "OrdenServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoMantenimientos_VehiculoId",
                table: "VehiculoMantenimientos",
                column: "VehiculoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facturas_EstadosFactura_EstadoFacturaId",
                table: "Facturas",
                column: "EstadoFacturaId",
                principalTable: "EstadosFactura",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_PrioridadesOrden_PrioridadOrdenId",
                table: "OrdenesServicio",
                column: "PrioridadOrdenId",
                principalTable: "PrioridadesOrden",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_TiposCombustible_TipoCombustibleId",
                table: "Vehiculos",
                column: "TipoCombustibleId",
                principalTable: "TiposCombustible",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculos_TiposTransmision_TipoTransmisionId",
                table: "Vehiculos",
                column: "TipoTransmisionId",
                principalTable: "TiposTransmision",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facturas_EstadosFactura_EstadoFacturaId",
                table: "Facturas");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_PrioridadesOrden_PrioridadOrdenId",
                table: "OrdenesServicio");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_TiposCombustible_TipoCombustibleId",
                table: "Vehiculos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculos_TiposTransmision_TipoTransmisionId",
                table: "Vehiculos");

            migrationBuilder.DropTable(
                name: "ConfiguracionesSistema");

            migrationBuilder.DropTable(
                name: "CorreosClientes");

            migrationBuilder.DropTable(
                name: "DetalleFacturas");

            migrationBuilder.DropTable(
                name: "DireccionesClientes");

            migrationBuilder.DropTable(
                name: "EntradasInventario");

            migrationBuilder.DropTable(
                name: "EstadosCita");

            migrationBuilder.DropTable(
                name: "EstadosFactura");

            migrationBuilder.DropTable(
                name: "EstadosOrden");

            migrationBuilder.DropTable(
                name: "HistorialAccesos");

            migrationBuilder.DropTable(
                name: "HistorialCitas");

            migrationBuilder.DropTable(
                name: "HistorialPreciosRepuestos");

            migrationBuilder.DropTable(
                name: "ImpuestosFacturas");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "OrdenServiciosExtras");

            migrationBuilder.DropTable(
                name: "PrioridadesOrden");

            migrationBuilder.DropTable(
                name: "RolPermisos");

            migrationBuilder.DropTable(
                name: "SalidasInventario");

            migrationBuilder.DropTable(
                name: "SesionesUsuarios");

            migrationBuilder.DropTable(
                name: "TelefonosClientes");

            migrationBuilder.DropTable(
                name: "TiposCombustible");

            migrationBuilder.DropTable(
                name: "TiposMovimientoInventario");

            migrationBuilder.DropTable(
                name: "TiposTransmision");

            migrationBuilder.DropTable(
                name: "VehiculoDocumentos");

            migrationBuilder.DropTable(
                name: "VehiculoFotos");

            migrationBuilder.DropTable(
                name: "VehiculoKilometrajes");

            migrationBuilder.DropTable(
                name: "VehiculoMantenimientos");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_TipoCombustibleId",
                table: "Vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_Vehiculos_TipoTransmisionId",
                table: "Vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesServicio_PrioridadOrdenId",
                table: "OrdenesServicio");

            migrationBuilder.DropIndex(
                name: "IX_Facturas_EstadoFacturaId",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "TipoCombustibleId",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "TipoTransmisionId",
                table: "Vehiculos");

            migrationBuilder.DropColumn(
                name: "PrioridadOrdenId",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "EstadoFacturaId",
                table: "Facturas");
        }
    }
}
