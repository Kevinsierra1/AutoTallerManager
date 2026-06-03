using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public class AuthMenu
{
    private readonly ApiService _api;

    public AuthMenu(ApiService api) => _api = api;

    public async Task<AuthResponse?> ShowAsync()
    {
        while (true)
        {
            try { AnsiConsole.Clear(); } catch { }

            AnsiConsole.Write(new FigletText("AutoTaller").Color(Color.Blue));
            AnsiConsole.Write(new FigletText("Manager").Color(Color.CadetBlue));
            AnsiConsole.Write(new Rule("[grey]Sistema de Gestión de Taller Automotriz[/]").RuleStyle("grey"));
            AnsiConsole.WriteLine();

            // ── Usuarios de prueba ──
            var hint = new Table().Border(TableBorder.None).HideHeaders().Expand();
            hint.AddColumn(""); hint.AddColumn(""); hint.AddColumn("");
            hint.AddRow("[grey]admin@autotaller.com[/]",    "[grey]Admin@123[/]",       "[red]Admin[/]");
            hint.AddRow("[grey]jefe@autotaller.com[/]",     "[grey]Jefe@123[/]",        "[yellow]Jefe de Taller[/]");
            hint.AddRow("[grey]mecanico@autotaller.com[/]", "[grey]Mecanico@123[/]",    "[cyan]Mecánico[/]");
            hint.AddRow("[grey]recepcion@autotaller.com[/]","[grey]Recepcion@123[/]",   "[green]Recepcionista[/]");
            hint.AddRow("[grey]cliente@autotaller.com[/]",  "[grey]Cliente@123[/]",     "[green]Cliente[/]");
            hint.AddRow("[grey]almacen@autotaller.com[/]",  "[grey]Almacen@123[/]",     "[fuchsia]Jefe Almacén[/]");
            hint.AddRow("[grey]bodega@autotaller.com[/]",   "[grey]Bodega@123[/]",      "[fuchsia]Jefe Bodega[/]");
            AnsiConsole.Write(new Panel(hint)
                .Header("[grey]  Usuarios disponibles[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("grey")));
            AnsiConsole.WriteLine();

            var opcion = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold cyan]  ¿Qué deseas hacer?[/]")
                    .HighlightStyle(new Style(Color.Cyan1))
                    .AddChoices("  Iniciar Sesión", "  Registrarse como Cliente", "  Salir"));

            if (opcion.Contains("Salir")) return null;

            if (opcion.Contains("Registrarse"))
            {
                var result = await RegistrarClienteAsync();
                if (result != null) return result;
                continue;
            }

            // ── Login ──
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("[bold]Iniciar Sesión[/]").RuleStyle("blue"));
            AnsiConsole.WriteLine();

            var email = AnsiConsole.Prompt(
                new TextPrompt<string>("[cyan]  Email:[/]")
                    .DefaultValue("admin@autotaller.com"));

            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("[cyan]  Contraseña:[/]")
                    .Secret()
                    .DefaultValue("Admin@123"));

            AnsiConsole.WriteLine();

            AuthResponse? auth = null;
            string? error = null;

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("cyan"))
                .StartAsync("[cyan]Autenticando...[/]", async ctx =>
                {
                    (auth, error) = await _api.LoginAsync(email, password);
                    ctx.Status("[green]Listo[/]");
                });

            if (auth != null)
            {
                _api.SetToken(auth.Token);
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("AutoTaller").Color(Color.Blue));

                (string rolColor, string rolIcon, string bienvenida) = auth.Roles.FirstOrDefault() switch
                {
                    "Admin"               => ("red",     "⚙",  "Acceso total al sistema"),
                    "JefeTaller"          => ("yellow",  "🔧", "Panel de Jefe de Taller"),
                    "Mecánico"            => ("cyan",    "🔩", "Panel de Mecánico"),
                    "MecanicoDiagnostico" => ("cyan",    "🔍", "Panel de Diagnóstico"),
                    "MecanicoArea"        => ("cyan",    "🔩", "Panel de Área"),
                    "Recepcionista"       => ("green",   "📋", "Panel de Recepción"),
                    "Cliente"             => ("green",   "🚗", "Portal del Cliente"),
                    "JefeAlmacen"         => ("fuchsia", "📦", "Panel de Almacén"),
                    "JefeBodega"          => ("fuchsia", "📦", "Panel de Bodega"),
                    _                     => ("white",   "👤", "Panel de usuario")
                };

                AnsiConsole.Write(new Panel(
                    $"[white]  Bienvenido, [bold]{Markup.Escape(auth.NombreCompleto)}[/][/]\n\n" +
                    $"  Rol: [{rolColor}]{rolIcon}  {Markup.Escape(auth.RolesStr)}[/]\n" +
                    $"  {bienvenida}")
                    .Header("[bold green]  ✓ Sesión Iniciada[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse(rolColor)));

                await Task.Delay(1500);
                return auth;
            }

            AnsiConsole.MarkupLine($"[red]  ✗ {Markup.Escape(error ?? "Error de autenticación")}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[grey]  Presiona Enter para intentar de nuevo...[/]");
            System.Console.ReadLine();
        }
    }

    // ── Registro de cliente ───────────────────────────────────────────────────

    private async Task<AuthResponse?> RegistrarClienteAsync()
    {
        try { AnsiConsole.Clear(); } catch { }
        AnsiConsole.Write(new Rule("[bold green]  Registro de Cliente[/]").RuleStyle("green"));
        AnsiConsole.WriteLine();

        // ── Datos personales ──
        AnsiConsole.MarkupLine("[bold]  Datos personales[/]");
        AnsiConsole.WriteLine();

        var nombres   = AnsiConsole.Prompt(new TextPrompt<string>("[cyan]  Nombres:[/]"));
        var apellidos = AnsiConsole.Prompt(new TextPrompt<string>("[cyan]  Apellidos:[/]"));

        var tipoDoc = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Tipo de documento:[/]")
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices("CC", "CE", "PA", "NIT"));

        var numDoc   = AnsiConsole.Prompt(new TextPrompt<string>("[cyan]  Número de documento:[/]"));
        var email    = AnsiConsole.Prompt(new TextPrompt<string>("[cyan]  Email:[/]"));
        var telefono = AnsiConsole.Prompt(new TextPrompt<string>("[cyan]  Teléfono (opcional):[/]").AllowEmpty());

        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan]  Contraseña:[/]")
                .Secret()
                .Validate(p => p.Length >= 6
                    ? ValidationResult.Success()
                    : ValidationResult.Error("Mínimo 6 caracteres")));

        var confirm = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan]  Confirmar contraseña:[/]")
                .Secret()
                .Validate(p => p == password
                    ? ValidationResult.Success()
                    : ValidationResult.Error("Las contraseñas no coinciden")));

        // ── Datos del vehículo ──
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]  Datos del vehículo[/]");
        AnsiConsole.WriteLine();

        // Cargar catálogos (endpoints públicos — no requieren login)
        List<CatalogoItem>? marcas = null;
        List<ColorItem>?    colores = null;

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("cyan"))
            .StartAsync("[cyan]Cargando catálogo...[/]", async ctx =>
            {
                marcas  = await _api.GetMarcasAsync();
                colores = await _api.GetColoresAsync();
                ctx.Status("[green]Listo[/]");
            });

        if (marcas == null || marcas.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]  No se pudo cargar el catálogo de marcas. Intenta más tarde.[/]");
            AnsiConsole.Markup("[grey]  Presiona Enter para volver...[/]");
            System.Console.ReadLine();
            return null;
        }

        // Seleccionar marca
        var marcaOpciones = marcas.Select(m => Markup.Escape(m.Nombre)).ToList();
        var marcaSel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Marca del vehículo:[/]")
                .PageSize(12)
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices(marcaOpciones));
        var marca = marcas[marcaOpciones.IndexOf(marcaSel)];

        // Cargar modelos de esa marca
        List<ModeloItem>? modelos = null;
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("cyan"))
            .StartAsync("[cyan]Cargando modelos...[/]", async ctx =>
            {
                modelos = await _api.GetModelosAsync(marca.Id);
                ctx.Status("[green]Listo[/]");
            });

        if (modelos == null || modelos.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]  No hay modelos disponibles para esta marca.[/]");
            AnsiConsole.Markup("[grey]  Presiona Enter para volver...[/]");
            System.Console.ReadLine();
            return null;
        }

        var modeloOpciones = modelos.Select(m => Markup.Escape(m.Nombre)).ToList();
        var modeloSel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Modelo:[/]")
                .PageSize(12)
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices(modeloOpciones));
        var modelo = modelos[modeloOpciones.IndexOf(modeloSel)];

        var anio = AnsiConsole.Prompt(
            new TextPrompt<int>("[cyan]  Año del vehículo:[/]")
                .DefaultValue(DateTime.Now.Year)
                .Validate(a => a >= 1970 && a <= DateTime.Now.Year + 1
                    ? ValidationResult.Success()
                    : ValidationResult.Error($"Año inválido (1970–{DateTime.Now.Year + 1})")));

        var placa = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan]  Placa (ej: ABC123):[/]")
                .Validate(p => p.Length >= 5
                    ? ValidationResult.Success()
                    : ValidationResult.Error("Mínimo 5 caracteres")));

        // Color (opcional)
        Guid? colorId = null;
        if (colores != null && colores.Count > 0)
        {
            var colorOpciones = colores.Select(c => Markup.Escape(c.Nombre)).ToList();
            colorOpciones.Insert(0, "No especificar");
            var colorSel = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[cyan]  Color del vehículo:[/]")
                    .PageSize(12)
                    .HighlightStyle(new Style(Color.Cyan1))
                    .AddChoices(colorOpciones));
            if (colorSel != "No especificar")
                colorId = colores[colorOpciones.IndexOf(colorSel) - 1].Id;
        }

        // ── Registrar ──
        AnsiConsole.WriteLine();
        AuthResponse? auth = null;
        string? error = null;

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("cyan"))
            .StartAsync("[cyan]Registrando...[/]", async ctx =>
            {
                (auth, error) = await _api.RegisterClienteAsync(new
                {
                    Email           = email,
                    Password        = password,
                    Nombres         = nombres,
                    Apellidos       = apellidos,
                    TipoDocumento   = tipoDoc,
                    NumeroDocumento = numDoc,
                    Telefono        = string.IsNullOrWhiteSpace(telefono) ? null : telefono,
                    Placa           = placa,
                    ModeloVehiculoId = modelo.Id,
                    Anio            = anio,
                    ColorId         = colorId,
                    Vin             = (string?)null
                });
                ctx.Status("[green]Listo[/]");
            });

        if (auth != null)
        {
            _api.SetToken(auth.Token);
            AnsiConsole.Clear();
            AnsiConsole.Write(new Panel(
                $"[white]Bienvenido,[/] [bold cyan]{Markup.Escape(auth.NombreCompleto)}[/]\n\n" +
                $"[grey]Tu cuenta ha sido creada. Puedes iniciar sesión cuando quieras\n" +
                $"para revisar y aprobar las órdenes de tu vehículo.[/]")
                .Header("[bold green]  ✓ Registro Exitoso[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("green")));
            await Task.Delay(2000);
            return auth;
        }

        AnsiConsole.MarkupLine($"[red]  ✗ {Markup.Escape(error ?? "Error al registrar")}[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[grey]  Presiona Enter para volver...[/]");
        System.Console.ReadLine();
        return null;
    }
}
