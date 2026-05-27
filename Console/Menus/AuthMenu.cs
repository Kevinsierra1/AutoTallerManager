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
                AnsiConsole.MarkupLine($"[green]  ✓ Bienvenido, [bold]{Markup.Escape(auth.NombreCompleto)}[/]! ([yellow]{Markup.Escape(auth.RolesStr)}[/])[/]");
                await Task.Delay(1200);
                return auth;
            }

            AnsiConsole.MarkupLine($"[red]  ✗ {Markup.Escape(error ?? "Error de autenticación")}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[grey]  Presiona Enter para intentar de nuevo...[/]");
            System.Console.ReadLine();
        }
    }
}
