using Spectre.Console;
using AutoTallerManager.ConsoleClient.Services;

namespace AutoTallerManager.ConsoleClient.Menus;

public class AuthMenu
{
    private readonly ApiService _api;

    public AuthMenu(ApiService api) => _api = api;

    public async Task<bool> LoginAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("AutoTaller").Color(Color.Blue));
        AnsiConsole.Write(new Rule("[yellow]Inicio de Sesión[/]").RuleStyle("grey"));

        var email = AnsiConsole.Ask<string>("[green]Email:[/]");
        var password = AnsiConsole.Prompt(new TextPrompt<string>("[green]Contraseña:[/]").Secret());

        AuthMenu? auth = null;
        await AnsiConsole.Status()
            .StartAsync("Autenticando...", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                var result = await _api.LoginAsync(email, password);
                if (result != null)
                    AnsiConsole.MarkupLine($"[green]✓ Bienvenido! Roles: {string.Join(", ", result.Roles)}[/]");
                else
                    AnsiConsole.MarkupLine("[red]✗ Credenciales inválidas.[/]");
            });

        return _api.IsAuthenticated;
    }
}
