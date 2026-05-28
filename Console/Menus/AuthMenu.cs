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
            AnsiConsole.Write(new Panel(hint)
                .Header("[grey]  Usuarios disponibles[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("grey")));
            AnsiConsole.WriteLine();

            var opcion = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold cyan]  ¿Qué deseas hacer?[/]")
                    .HighlightStyle(new Style(Color.Cyan1))
                    .AddChoices("  Iniciar Sesión", "  Salir"));

            if (opcion.Contains("Salir")) return null;

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
}
