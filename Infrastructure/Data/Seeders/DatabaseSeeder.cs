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
            await SeedTiposDocumentoAsync(context);
            await SeedMarcasYModelosAsync(context);
            await SeedColoresAsync(context);
            await SeedCategoriasRepuestoAsync(context);
            await SeedTiposServicioAsync(context);
            await SeedMetodosPagoAsync(context);
            await SeedAdminUserAsync(context);
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
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Nombre = "Cliente", Descripcion = "Cliente del taller" }
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

        var admin = new Usuario
        {
            Id = adminId,
            Email = "admin@autotaller.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Nombres = "Administrador",
            Apellidos = "Sistema",
            Activo = true
        };

        ctx.Usuarios.Add(admin);
        ctx.UsuarioRoles.Add(new UsuarioRol { UsuarioId = adminId, RolId = rolAdminId });
    }
}
