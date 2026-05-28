using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            await SeedRolesAsync(context);
            await SeedAreasTallerAsync(context);
            await SeedTiposDocumentoAsync(context);
            await SeedMarcasYModelosAsync(context);
            await SeedColoresAsync(context);
            await SeedCategoriasRepuestoAsync(context);
            await SeedTiposServicioAsync(context);
            await SeedMetodosPagoAsync(context);
            await SeedAdminUserAsync(context);
            await SeedUsuariosPruebaAsync(context);
            await context.SaveChangesAsync();
            logger.LogInformation("Datos de prueba cargados exitosamente.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al cargar datos de prueba.");
        }
    }

    private static async Task SeedRolesAsync(ApplicationDbContext ctx)
    {
        if (await ctx.Roles.AnyAsync()) return;
        ctx.Roles.AddRange(
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Nombre = "Admin", Descripcion = "Administrador del sistema" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Nombre = "Mecánico", Descripcion = "Técnico mecánico" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Nombre = "Recepcionista", Descripcion = "Atención al cliente" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Nombre = "Cliente", Descripcion = "Cliente del taller" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Nombre = "JefeTaller", Descripcion = "Jefe de taller — aprueba presupuestos y mini-órdenes" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Nombre = "MecanicoDiagnostico", Descripcion = "Mecánico especialista en diagnóstico" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Nombre = "MecanicoArea", Descripcion = "Mecánico asignado a un área específica" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), Nombre = "JefeAlmacen", Descripcion = "Jefe de almacén — aprueba solicitudes de inventario" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), Nombre = "JefeBodega", Descripcion = "Jefe de bodega — gestiona transferencias de stock" }
        );
    }

    private static async Task SeedAreasTallerAsync(ApplicationDbContext ctx)
    {
        if (await ctx.AreasTaller.AnyAsync()) return;
        ctx.AreasTaller.AddRange(
            new Domain.Entities.AreaTaller { Id = Guid.Parse("A0000000-0000-0000-0000-000000000001"), Nombre = "Motor", Tipo = Domain.Enums.TipoArea.Motor, Descripcion = "Reparación y mantenimiento de motor" },
            new Domain.Entities.AreaTaller { Id = Guid.Parse("A0000000-0000-0000-0000-000000000002"), Nombre = "Frenos", Tipo = Domain.Enums.TipoArea.Frenos, Descripcion = "Sistema de frenos" },
            new Domain.Entities.AreaTaller { Id = Guid.Parse("A0000000-0000-0000-0000-000000000003"), Nombre = "Suspensión", Tipo = Domain.Enums.TipoArea.Suspension, Descripcion = "Suspensión y dirección" },
            new Domain.Entities.AreaTaller { Id = Guid.Parse("A0000000-0000-0000-0000-000000000004"), Nombre = "Eléctrico", Tipo = Domain.Enums.TipoArea.Electrico, Descripcion = "Sistema eléctrico y electrónico" },
            new Domain.Entities.AreaTaller { Id = Guid.Parse("A0000000-0000-0000-0000-000000000005"), Nombre = "Transmisión", Tipo = Domain.Enums.TipoArea.Transmision, Descripcion = "Caja de cambios y transmisión" },
            new Domain.Entities.AreaTaller { Id = Guid.Parse("A0000000-0000-0000-0000-000000000006"), Nombre = "Pintura", Tipo = Domain.Enums.TipoArea.Pintura, Descripcion = "Carrocería y pintura" },
            new Domain.Entities.AreaTaller { Id = Guid.Parse("A0000000-0000-0000-0000-000000000007"), Nombre = "Diagnóstico", Tipo = Domain.Enums.TipoArea.Diagnostico, Descripcion = "Diagnóstico computarizado" }
        );
    }

    private static async Task SeedTiposDocumentoAsync(ApplicationDbContext ctx)
    {
        if (await ctx.TiposDocumento.AnyAsync()) return;
        ctx.TiposDocumento.AddRange(
            new TipoDocumento { Nombre = "Cédula de Ciudadanía", Abreviatura = "CC" },
            new TipoDocumento { Nombre = "Cédula de Extranjería", Abreviatura = "CE" },
            new TipoDocumento { Nombre = "Pasaporte", Abreviatura = "PA" },
            new TipoDocumento { Nombre = "NIT", Abreviatura = "NIT" }
        );
    }

    private static async Task SeedMarcasYModelosAsync(ApplicationDbContext ctx)
    {
        if (await ctx.Marcas.AnyAsync()) return;
        var marcas = new[]
        {
            new Marca { Nombre = "Toyota", Modelos = new List<ModeloVehiculo> { new() { Nombre = "Corolla" }, new() { Nombre = "Hilux" }, new() { Nombre = "RAV4" } } },
            new Marca { Nombre = "Chevrolet", Modelos = new List<ModeloVehiculo> { new() { Nombre = "Spark" }, new() { Nombre = "Captiva" }, new() { Nombre = "Silverado" } } },
            new Marca { Nombre = "Ford", Modelos = new List<ModeloVehiculo> { new() { Nombre = "Ranger" }, new() { Nombre = "Explorer" }, new() { Nombre = "Mustang" } } },
            new Marca { Nombre = "Nissan", Modelos = new List<ModeloVehiculo> { new() { Nombre = "Frontier" }, new() { Nombre = "Kicks" }, new() { Nombre = "X-Trail" } } },
            new Marca { Nombre = "Hyundai", Modelos = new List<ModeloVehiculo> { new() { Nombre = "Tucson" }, new() { Nombre = "Santa Fe" }, new() { Nombre = "Accent" } } },
            new Marca { Nombre = "Mazda", Modelos = new List<ModeloVehiculo> { new() { Nombre = "CX-5" }, new() { Nombre = "3" }, new() { Nombre = "6" } } },
        };
        ctx.Marcas.AddRange(marcas);
    }

    private static async Task SeedColoresAsync(ApplicationDbContext ctx)
    {
        if (await ctx.Colores.AnyAsync()) return;
        ctx.Colores.AddRange(
            new Color { Nombre = "Blanco", CodigoHex = "#FFFFFF" },
            new Color { Nombre = "Negro", CodigoHex = "#000000" },
            new Color { Nombre = "Gris", CodigoHex = "#808080" },
            new Color { Nombre = "Plata", CodigoHex = "#C0C0C0" },
            new Color { Nombre = "Rojo", CodigoHex = "#FF0000" },
            new Color { Nombre = "Azul", CodigoHex = "#0000FF" },
            new Color { Nombre = "Verde", CodigoHex = "#008000" },
            new Color { Nombre = "Amarillo", CodigoHex = "#FFFF00" }
        );
    }

    private static async Task SeedCategoriasRepuestoAsync(ApplicationDbContext ctx)
    {
        if (await ctx.CategoriasRepuesto.AnyAsync()) return;
        ctx.CategoriasRepuesto.AddRange(
            new CategoriaRepuesto { Nombre = "Filtros", Descripcion = "Filtros de aceite, aire, combustible" },
            new CategoriaRepuesto { Nombre = "Frenos", Descripcion = "Pastillas, discos, tambores" },
            new CategoriaRepuesto { Nombre = "Motor", Descripcion = "Partes internas de motor" },
            new CategoriaRepuesto { Nombre = "Suspensión", Descripcion = "Amortiguadores, resortes, rótulas" },
            new CategoriaRepuesto { Nombre = "Eléctrico", Descripcion = "Batería, alternador, cables" },
            new CategoriaRepuesto { Nombre = "Lubricantes", Descripcion = "Aceites y líquidos" }
        );
    }

    private static async Task SeedTiposServicioAsync(ApplicationDbContext ctx)
    {
        if (await ctx.TiposServicio.AnyAsync()) return;
        ctx.TiposServicio.AddRange(
            new TipoServicio { Nombre = "Mantenimiento Preventivo", PrecioBase = 150000 },
            new TipoServicio { Nombre = "Cambio de Aceite", PrecioBase = 80000 },
            new TipoServicio { Nombre = "Revisión de Frenos", PrecioBase = 120000 },
            new TipoServicio { Nombre = "Sistema Eléctrico", PrecioBase = 200000 },
            new TipoServicio { Nombre = "Alineación y Balanceo", PrecioBase = 90000 },
            new TipoServicio { Nombre = "Diagnóstico General", PrecioBase = 50000 }
        );
    }

    private static async Task SeedMetodosPagoAsync(ApplicationDbContext ctx)
    {
        if (await ctx.MetodosPago.AnyAsync()) return;
        ctx.MetodosPago.AddRange(
            new MetodoPago { Nombre = "Efectivo" },
            new MetodoPago { Nombre = "Tarjeta Débito" },
            new MetodoPago { Nombre = "Tarjeta Crédito" },
            new MetodoPago { Nombre = "Transferencia Bancaria" },
            new MetodoPago { Nombre = "Nequi/Daviplata" }
        );
    }

    private static async Task SeedAdminUserAsync(ApplicationDbContext ctx)
    {
        if (await ctx.Usuarios.AnyAsync(u => u.Email == "admin@autotaller.com")) return;

        var adminId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var rolAdminId = Guid.Parse("10000000-0000-0000-0000-000000000001");

        ctx.Usuarios.Add(new Usuario
        {
            Id = adminId,
            Email = "admin@autotaller.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Nombres = "Administrador",
            Apellidos = "Sistema",
            Activo = true
        });
        ctx.UsuarioRoles.Add(new UsuarioRol { UsuarioId = adminId, RolId = rolAdminId });
    }

    private static async Task SeedUsuariosPruebaAsync(ApplicationDbContext ctx)
    {
        // Solo crea si no existe ninguno de los usuarios de prueba
        if (await ctx.Usuarios.AnyAsync(u => u.Email == "jefe@autotaller.com")) return;

        var usuarios = new[]
        {
            (Id: Guid.Parse("20000000-0000-0000-0000-000000000002"),
             Email: "jefe@autotaller.com",       Pass: "Jefe@123",
             Nombres: "Carlos",   Apellidos: "Ramírez",
             RolId: Guid.Parse("10000000-0000-0000-0000-000000000005")),  // JefeTaller

            (Id: Guid.Parse("20000000-0000-0000-0000-000000000003"),
             Email: "mecanico@autotaller.com",   Pass: "Mecanico@123",
             Nombres: "Andrés",   Apellidos: "Torres",
             RolId: Guid.Parse("10000000-0000-0000-0000-000000000002")),  // Mecánico

            (Id: Guid.Parse("20000000-0000-0000-0000-000000000004"),
             Email: "recepcion@autotaller.com",  Pass: "Recepcion@123",
             Nombres: "Laura",    Apellidos: "Gómez",
             RolId: Guid.Parse("10000000-0000-0000-0000-000000000003")),  // Recepcionista

            (Id: Guid.Parse("20000000-0000-0000-0000-000000000005"),
             Email: "cliente@autotaller.com",    Pass: "Cliente@123",
             Nombres: "Juan",     Apellidos: "Pérez",
             RolId: Guid.Parse("10000000-0000-0000-0000-000000000004")),  // Cliente

            (Id: Guid.Parse("20000000-0000-0000-0000-000000000006"),
             Email: "almacen@autotaller.com",    Pass: "Almacen@123",
             Nombres: "Miguel",   Apellidos: "Vargas",
             RolId: Guid.Parse("10000000-0000-0000-0000-000000000008")),  // JefeAlmacen

            (Id: Guid.Parse("20000000-0000-0000-0000-000000000007"),
             Email: "diagnostico@autotaller.com", Pass: "Diagnostico@123",
             Nombres: "Felipe",   Apellidos: "Castro",
             RolId: Guid.Parse("10000000-0000-0000-0000-000000000006")),  // MecanicoDiagnostico
        };

        foreach (var u in usuarios)
        {
            ctx.Usuarios.Add(new Usuario
            {
                Id = u.Id,
                Email = u.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(u.Pass),
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,
                Activo = true
            });
            ctx.UsuarioRoles.Add(new UsuarioRol { UsuarioId = u.Id, RolId = u.RolId });
        }
    }
}
