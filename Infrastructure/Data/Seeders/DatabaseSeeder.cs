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
            await SeedMecanicosAsync(context);
            // Guardar primero para que SeedClientesAsync pueda consultar marcas/modelos ya persistidos
            await context.SaveChangesAsync();
            await SeedClientesAsync(context);
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
        var roles = new[]
        {
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Nombre = "Admin",                Descripcion = "Administrador del sistema" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Nombre = "Mecánico",             Descripcion = "Técnico mecánico" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Nombre = "Recepcionista",        Descripcion = "Atención al cliente" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Nombre = "Cliente",              Descripcion = "Cliente del taller" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Nombre = "JefeTaller",           Descripcion = "Jefe de taller — aprueba presupuestos y mini-órdenes" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Nombre = "MecanicoDiagnostico",  Descripcion = "Mecánico especialista en diagnóstico" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Nombre = "MecanicoArea",         Descripcion = "Mecánico asignado a un área específica" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), Nombre = "JefeAlmacen",          Descripcion = "Jefe de almacén — aprueba solicitudes de inventario" },
            new Rol { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), Nombre = "JefeBodega",           Descripcion = "Jefe de bodega — gestiona transferencias de stock" },
        };

        var idsExistentes = await ctx.Roles.Select(r => r.Id).ToListAsync();
        ctx.Roles.AddRange(roles.Where(r => !idsExistentes.Contains(r.Id)));
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
        // Carga aditiva: agrega las marcas/modelos que falten sin tocar los existentes.
        var nombresExistentes = new HashSet<string>(
            await ctx.Marcas.Select(m => m.Nombre).ToListAsync());

        static List<ModeloVehiculo> M(params string[] nombres) =>
            nombres.Select(n => new ModeloVehiculo { Nombre = n }).ToList();

        var marcas = new List<Marca>
        {
            // ── Marcas con mayor volumen de ventas en Colombia ────────────────
            new() { Nombre = "Chevrolet", Modelos = M(
                "Spark", "Onix", "Tracker", "Captiva", "Equinox",
                "Trailblazer", "Blazer", "Silverado", "Colorado", "S10",
                "Sail", "Aveo", "Cruze", "Malibu", "Suburban") },

            new() { Nombre = "Renault", Modelos = M(
                "Logan", "Sandero", "Duster", "Koleos", "Kwid",
                "Stepway", "Symbol", "Fluence", "Megane", "Clio",
                "Captur", "Oroch", "Alaskan", "Triber", "Kangoo") },

            new() { Nombre = "Kia", Modelos = M(
                "Picanto", "Rio", "Sonet", "Sportage", "Stonic",
                "Seltos", "Sorento", "Carnival", "EV6", "Stinger",
                "Soul", "Cerato", "Telluride", "Forte", "Niro") },

            new() { Nombre = "Toyota", Modelos = M(
                "Corolla", "Corolla Cross", "Hilux", "RAV4", "Land Cruiser",
                "Prado", "Fortuner", "Camry", "Yaris", "Prius",
                "Rush", "Hiace", "Avanza", "C-HR", "Sequoia") },

            new() { Nombre = "Mazda", Modelos = M(
                "Mazda 2", "Mazda 3", "Mazda 6", "CX-3", "CX-30",
                "CX-5", "CX-50", "CX-9", "MX-5", "BT-50") },

            new() { Nombre = "Hyundai", Modelos = M(
                "Grand i10", "i20", "Accent", "Elantra", "Tucson",
                "Santa Fe", "Creta", "Kona", "Ioniq 5", "Ioniq 6",
                "Venue", "Palisade", "Staria", "H-1", "H100") },

            new() { Nombre = "Nissan", Modelos = M(
                "Versa", "Sentra", "Altima", "March", "Kicks",
                "X-Trail", "Murano", "Pathfinder", "Frontier", "Navara",
                "NP300", "Leaf", "Qashqai", "Terra", "Armada") },

            new() { Nombre = "Suzuki", Modelos = M(
                "Alto", "Celerio", "Ignis", "S-Presso", "Swift",
                "Baleno", "Dzire", "Ciaz", "Vitara", "Grand Vitara",
                "Jimny", "Ertiga", "XL7", "Across", "SX4 S-Cross") },

            new() { Nombre = "Volkswagen", Modelos = M(
                "Polo", "Vento", "Golf", "Jetta", "Passat",
                "Tiguan", "Touareg", "T-Cross", "T-Roc", "Taos",
                "Teramont", "Amarok", "Arteon", "ID.4", "Saveiro") },

            new() { Nombre = "Ford", Modelos = M(
                "EcoSport", "Territory", "Explorer", "Expedition", "Bronco",
                "Ranger", "F-150", "Escape", "Edge", "Mustang",
                "Maverick", "Transit", "Figo", "Ka", "Fusion") },

            // ── Marcas premium / europeas ─────────────────────────────────────
            new() { Nombre = "BMW", Modelos = M(
                "Serie 1", "Serie 2", "Serie 3", "Serie 5", "Serie 7",
                "X1", "X2", "X3", "X5", "X6",
                "X7", "iX", "i4", "M3", "M5") },

            new() { Nombre = "Mercedes-Benz", Modelos = M(
                "Clase A", "Clase C", "Clase E", "Clase S", "GLA",
                "GLB", "GLC", "GLE", "GLS", "AMG GT",
                "CLA", "EQB", "EQC", "Sprinter", "Vito") },

            new() { Nombre = "Audi", Modelos = M(
                "A1", "A3", "A4", "A5", "A6",
                "Q2", "Q3", "Q5", "Q7", "Q8",
                "e-tron", "RS3", "RS6", "TT", "R8") },

            new() { Nombre = "Volvo", Modelos = M(
                "XC40", "XC60", "XC90", "S60", "S90",
                "V60", "C40", "EX30", "EX90") },

            new() { Nombre = "Land Rover", Modelos = M(
                "Defender", "Discovery", "Discovery Sport",
                "Range Rover", "Range Rover Sport", "Range Rover Evoque",
                "Range Rover Velar", "Freelander") },

            new() { Nombre = "Peugeot", Modelos = M(
                "208", "308", "408", "508", "2008",
                "3008", "5008", "Partner", "Traveller", "Rifter") },

            new() { Nombre = "Citroën", Modelos = M(
                "C3", "C3 Aircross", "C4", "C5 Aircross",
                "Berlingo", "Jumpy", "SpaceTourer") },

            new() { Nombre = "Fiat", Modelos = M(
                "Mobi", "Cronos", "Pulse", "Fastback",
                "Argo", "Doblo", "Ducato", "Toro") },

            // ── Japonesas / coreanas adicionales ─────────────────────────────
            new() { Nombre = "Honda", Modelos = M(
                "Fit", "City", "Civic", "Accord", "Insight",
                "HR-V", "CR-V", "Passport", "Pilot", "Ridgeline",
                "WR-V", "ZR-V", "BR-V", "Jazz", "e:Ny1") },

            new() { Nombre = "Mitsubishi", Modelos = M(
                "Mirage", "Attrage", "Lancer", "Eclipse Cross", "Outlander",
                "ASX", "Montero Sport", "Montero", "L200", "Pajero") },

            new() { Nombre = "Subaru", Modelos = M(
                "Impreza", "Legacy", "Outback", "Forester",
                "XV", "BRZ", "Crosstrek", "Ascent", "Solterra") },

            new() { Nombre = "Isuzu", Modelos = M(
                "D-Max", "MU-X", "TF", "NPR", "NMR",
                "NKR", "FRR", "FVR") },

            new() { Nombre = "Jeep", Modelos = M(
                "Renegade", "Compass", "Commander",
                "Grand Cherokee", "Wrangler", "Gladiator", "Avenger") },

            new() { Nombre = "RAM", Modelos = M(
                "700", "1000", "2000", "3500",
                "1500", "2500", "3500 HD", "ProMaster") },

            new() { Nombre = "Dodge", Modelos = M(
                "Journey", "Durango", "Charger",
                "Challenger", "Grand Caravan") },

            // ── Marcas chinas con presencia comercial en Colombia ─────────────
            new() { Nombre = "JAC", Modelos = M(
                "J2", "J3", "J4", "J7", "S1",
                "S2", "S3", "S4", "E10X", "T6",
                "T8", "X200", "N55", "N75", "Sei7") },

            new() { Nombre = "Chery", Modelos = M(
                "QQ", "Arrizo 3", "Arrizo 5", "Arrizo 6", "Tiggo 2",
                "Tiggo 4", "Tiggo 5X", "Tiggo 7", "Tiggo 8", "Omoda 5") },

            new() { Nombre = "Geely", Modelos = M(
                "Emgrand", "Coolray", "Azkarra", "Okavango",
                "Atlas", "Geometry C", "Geometry E") },

            new() { Nombre = "BYD", Modelos = M(
                "Dolphin", "Seal", "Atto 3", "Han",
                "Tang", "Song Plus", "Yuan Plus", "King") },

            new() { Nombre = "Haval", Modelos = M(
                "H1", "H2", "H6", "H9",
                "Jolion", "Dargo", "Big Dog") },

            new() { Nombre = "DFSK", Modelos = M(
                "Glory 330", "Glory 500", "Glory 580",
                "Glory 560", "C35", "C37") },

            new() { Nombre = "ZNA", Modelos = M(
                "Rich", "Rich 6", "Dolphin",
                "Grand Tiger", "Pickup") },

            new() { Nombre = "Foton", Modelos = M(
                "Tunland", "Sauvana", "View C2",
                "Toano", "M3", "Aumark") },
        };

        // Solo inserta las marcas que aún no existen (por nombre)
        var nuevas = marcas.Where(m => !nombresExistentes.Contains(m.Nombre)).ToList();
        if (nuevas.Count > 0)
        {
            ctx.Marcas.AddRange(nuevas);
            Console.WriteLine($"[Seeder] Agregando {nuevas.Count} marca(s) nueva(s): {string.Join(", ", nuevas.Select(m => m.Nombre))}");
        }
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

            (Id: Guid.Parse("20000000-0000-0000-0000-000000000008"),
             Email: "bodega@autotaller.com",       Pass: "Bodega@123",
             Nombres: "Roberto",  Apellidos: "Mendoza",
             RolId: Guid.Parse("10000000-0000-0000-0000-000000000009")),  // JefeBodega

            (Id: Guid.Parse("20000000-0000-0000-0000-000000000009"),
             Email: "mecanicoArea@autotaller.com", Pass: "MecArea@123",
             Nombres: "Sofía",    Apellidos: "Ruiz",
             RolId: Guid.Parse("10000000-0000-0000-0000-000000000007")),  // MecanicoArea
        };

        var emailsExistentes = await ctx.Usuarios
            .Where(u => usuarios.Select(x => x.Email).Contains(u.Email))
            .Select(u => u.Email)
            .ToListAsync();

        foreach (var u in usuarios.Where(u => !emailsExistentes.Contains(u.Email)))
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

    private static async Task SeedMecanicosAsync(ApplicationDbContext ctx)
    {
        if (await ctx.Empleados.AnyAsync()) return;

        // TipoEmpleado: 0=Mecanico, 1=Electrico, 5=MecanicoDiagnostico, 6=MecanicoArea
        var mecanicos = new[]
        {
            (Id: "30000000-0000-0000-0000-000000000001", Nombres: "Juan",        Apellidos: "Rodríguez",  Tipo: 0, Esp: "Motor y carburación"),
            (Id: "30000000-0000-0000-0000-000000000002", Nombres: "Pedro",       Apellidos: "Martínez",   Tipo: 0, Esp: "Transmisión manual"),
            (Id: "30000000-0000-0000-0000-000000000003", Nombres: "Luis",        Apellidos: "Hernández",  Tipo: 0, Esp: "Frenos y ABS"),
            (Id: "30000000-0000-0000-0000-000000000004", Nombres: "Carlos",      Apellidos: "Gómez",      Tipo: 0, Esp: "Suspensión y dirección"),
            (Id: "30000000-0000-0000-0000-000000000005", Nombres: "Daniel",      Apellidos: "Medina",     Tipo: 0, Esp: "Alineación y balanceo"),
            (Id: "30000000-0000-0000-0000-000000000006", Nombres: "Héctor",      Apellidos: "Flores",     Tipo: 0, Esp: "Hidráulica"),
            (Id: "30000000-0000-0000-0000-000000000007", Nombres: "Óscar",       Apellidos: "Gutiérrez",  Tipo: 0, Esp: "Soldadura automotriz"),
            (Id: "30000000-0000-0000-0000-000000000008", Nombres: "Felipe",      Apellidos: "Castro",     Tipo: 1, Esp: "Sistemas eléctricos"),
            (Id: "30000000-0000-0000-0000-000000000009", Nombres: "Eduardo",     Apellidos: "Morales",    Tipo: 1, Esp: "Aire acondicionado automotriz"),
            (Id: "30000000-0000-0000-0000-000000000010", Nombres: "Raúl",        Apellidos: "Peña",       Tipo: 1, Esp: "Iluminación y sensores"),
            (Id: "30000000-0000-0000-0000-000000000011", Nombres: "Miguel",      Apellidos: "Torres",     Tipo: 5, Esp: "Inyección electrónica"),
            (Id: "30000000-0000-0000-0000-000000000012", Nombres: "Roberto",     Apellidos: "Sánchez",    Tipo: 5, Esp: "Escáner y diagnóstico OBD"),
            (Id: "30000000-0000-0000-0000-000000000013", Nombres: "Manuel",      Apellidos: "Reyes",      Tipo: 5, Esp: "Sistema de refrigeración"),
            (Id: "30000000-0000-0000-0000-000000000014", Nombres: "Gustavo",     Apellidos: "Álvarez",    Tipo: 5, Esp: "Sistemas de seguridad activa"),
            (Id: "30000000-0000-0000-0000-000000000015", Nombres: "Andrés",      Apellidos: "Vargas",     Tipo: 6, Esp: "Transmisión automática"),
            (Id: "30000000-0000-0000-0000-000000000016", Nombres: "Diego",       Apellidos: "Ramírez",    Tipo: 6, Esp: "Sistema de escape"),
            (Id: "30000000-0000-0000-0000-000000000017", Nombres: "Sebastián",   Apellidos: "López",      Tipo: 6, Esp: "Pintura y acabados"),
            (Id: "30000000-0000-0000-0000-000000000018", Nombres: "Cristian",    Apellidos: "Jiménez",    Tipo: 6, Esp: "Carrocería y chasis"),
            (Id: "30000000-0000-0000-0000-000000000019", Nombres: "Alejandro",   Apellidos: "Cruz",       Tipo: 6, Esp: "Neumáticos y rines"),
            (Id: "30000000-0000-0000-0000-000000000020", Nombres: "Ricardo",     Apellidos: "Ortiz",      Tipo: 6, Esp: "Planchado y latonería"),
        };

        foreach (var m in mecanicos)
        {
            ctx.Empleados.Add(new Empleado
            {
                Id              = Guid.Parse(m.Id),
                Nombres         = m.Nombres,
                Apellidos       = m.Apellidos,
                NumeroDocumento = $"DOC-{m.Id[^3..]}",
                TipoEmpleado    = (Domain.Enums.TipoEmpleadoEnum)m.Tipo,
                Especialidad    = m.Esp,
                Activo          = true
            });
        }
    }

    private static async Task SeedClientesAsync(ApplicationDbContext ctx)
    {
        if (await ctx.Clientes.AnyAsync()) return;

        // Obtener el tipo documento CC para asignarlo
        var tipoCC = await ctx.TiposDocumento.FirstOrDefaultAsync(t => t.Abreviatura == "CC");

        // Relacionar el usuario "cliente@autotaller.com" con su perfil
        var usuarioCliente = await ctx.Usuarios
            .FirstOrDefaultAsync(u => u.Email == "cliente@autotaller.com");

        // Perfil de cliente para Juan Pérez
        var juan = new Cliente
        {
            Nombres         = "Juan",
            Apellidos       = "Pérez",
            TipoDocumentoId = tipoCC?.Id,
            NumeroDocumento = "12345678",
            Email           = "cliente@autotaller.com",
            Telefono        = "3001234567",
            Activo          = true,
            UsuarioId       = usuarioCliente?.Id,
        };

        // Clientes adicionales de prueba
        var clientes = new[]
        {
            juan,
            new Cliente { Nombres = "María",   Apellidos = "García",    TipoDocumentoId = tipoCC?.Id, NumeroDocumento = "23456789", Email = "maria.garcia@ejemplo.com",    Telefono = "3109876543", Activo = true },
            new Cliente { Nombres = "Carlos",   Apellidos = "Martínez",  TipoDocumentoId = tipoCC?.Id, NumeroDocumento = "34567890", Email = "carlos.martinez@ejemplo.com", Telefono = "3207654321", Activo = true },
            new Cliente { Nombres = "Luisa",    Apellidos = "Rodríguez", TipoDocumentoId = tipoCC?.Id, NumeroDocumento = "45678901", Email = "luisa.rodriguez@ejemplo.com", Telefono = "3156789012", Activo = true },
            new Cliente { Nombres = "Andrés",   Apellidos = "López",     TipoDocumentoId = tipoCC?.Id, NumeroDocumento = "56789012", Email = "andres.lopez@ejemplo.com",    Telefono = "3004567890", Activo = true },
        };
        ctx.Clientes.AddRange(clientes);

        // Agregar un vehículo de prueba para Juan Pérez
        // Buscar el modelo "Corolla" de Toyota
        var modeloCorolla = await ctx.Marcas
            .Where(m => m.Nombre == "Toyota")
            .SelectMany(m => m.Modelos!)
            .FirstOrDefaultAsync(m => m.Nombre == "Corolla");

        if (modeloCorolla != null)
        {
            var vehiculo = new Vehiculo
            {
                Placa           = "ABC123",
                ModeloVehiculoId = modeloCorolla.Id,
                Anio            = 2020,
                KilometrajeActual = 45000,
                Activo          = true,
            };
            ctx.Vehiculos.Add(vehiculo);

            // Relacionar el vehículo con Juan Pérez después del SaveChanges
            // Usamos una entidad intermedia
            var propietario = new VehiculoPropietario
            {
                VehiculoId  = vehiculo.Id,
                ClienteId   = juan.Id,
                FechaInicio = DateTime.UtcNow.AddYears(-2),
                Activo      = true,
            };
            ctx.VehiculoPropietarios.Add(propietario);
        }
    }
}
