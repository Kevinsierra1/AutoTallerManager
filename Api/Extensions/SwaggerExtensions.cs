using Microsoft.OpenApi.Models;

namespace Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AutoTallerManager API",
                Version = "v1",
                Description = """
                    ## Sistema Empresarial de Gestión de Talleres Automotrices

                    API REST con 56 endpoints organizados en los siguientes módulos:

                    | Módulo | Tablas |
                    |--------|--------|
                    | **Catálogos** | Marcas, Modelos, Colores, TiposCombustible, TiposTransmision, EstadosOrden, EstadosCita, EstadosFactura, PrioridadesOrden, TiposMovInventario |
                    | **Seguridad** | Usuarios, Roles, Permisos, RolPermisos, SesionesUsuarios, HistorialAccesos |
                    | **Clientes** | Clientes, Direcciones, Teléfonos, Correos |
                    | **Vehículos** | Vehículos, Propietarios, Kilométrajes, Fotos, Mantenimientos, Documentos |
                    | **Agenda** | Citas, HistorialCitas |
                    | **Inventario** | Repuestos, Proveedores, Movimientos, Entradas, Salidas, HistorialPrecios |
                    | **Órdenes** | OrdenesServicio, Detalles, ManosObra, Extras, HistorialEstados |
                    | **Facturación** | Facturas, Detalles, Pagos, Impuestos |
                    | **Auditoría** | Auditorias, LogsErrores, Notificaciones, ConfiguracionesSistema |

                    ### Autenticación
                    Usa JWT Bearer. Obtén el token con `POST /api/auth/login` y colócalo en el botón **Authorize**.
                    """,
                Contact = new OpenApiContact
                {
                    Name = "AutoTallerManager",
                    Email = "soporte@autotaller.com"
                },
                License = new OpenApiLicense
                {
                    Name = "Uso Empresarial"
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization. Formato: **Bearer {token}**",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });

            c.TagActionsBy(api =>
            {
                if (api.GroupName != null) return new[] { api.GroupName };
                var controllerName = api.ActionDescriptor.RouteValues["controller"];
                return new[] { controllerName ?? "Default" };
            });

        });

        return services;
    }
}
