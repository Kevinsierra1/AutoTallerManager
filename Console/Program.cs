using System.Text.Json;
using AutoTaller.Console.Services;
using AutoTaller.Console.Menus;
using Spectre.Console;

// ── Configuración ─────────────────────────────────────────────────────────────

string baseUrl = "http://localhost:5000";

try
{
    var cfgFile = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    if (File.Exists(cfgFile))
    {
        var doc = JsonDocument.Parse(File.ReadAllText(cfgFile));
        if (doc.RootElement.TryGetProperty("ApiBaseUrl", out var prop))
            baseUrl = prop.GetString() ?? baseUrl;
    }
}
catch { /* usa el default */ }

// ── Inicio ────────────────────────────────────────────────────────────────────

var api = new ApiService(baseUrl);

try { AnsiConsole.Clear(); } catch { }

// Verificar conexión con la API
bool apiOk = false;
await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .SpinnerStyle(Style.Parse("cyan"))
    .StartAsync("[cyan]Conectando con la API...[/]", async ctx =>
    {
        apiOk = await api.PingAsync();
        ctx.Status(apiOk ? "[green]Conectado[/]" : "[red]Sin conexión[/]");
        await Task.Delay(500);
    });

if (!apiOk)
{
    AnsiConsole.Clear();
    AnsiConsole.Write(new FigletText("AutoTaller").Color(Color.Blue));
    AnsiConsole.WriteLine();
    AnsiConsole.Write(new Panel(
        $"[red]No se pudo conectar con la API.[/]\n\n" +
        $"URL configurada: [white]{baseUrl}[/]\n\n" +
        $"Asegúrate de que la API esté corriendo:\n" +
        $"[grey]  cd Api && dotnet run[/]\n\n" +
        $"Luego vuelve a ejecutar la consola.")
        .Header("[red]  Error de Conexión[/]")
        .Border(BoxBorder.Rounded)
        .BorderStyle(Style.Parse("red")));

    AnsiConsole.WriteLine();
    AnsiConsole.Markup("[grey]Presiona Enter para salir...[/]");
    System.Console.ReadLine();
    return;
}

// ── Loop de sesiones — vuelve al login al cerrar sesión ───────────────────────

var authMenu = new AuthMenu(api);

while (true)
{
    var authResult = await authMenu.ShowAsync();

    // El usuario eligió "Salir" desde la pantalla de login
    if (authResult == null)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("Hasta luego").Color(Color.Grey));
        AnsiConsole.MarkupLine("\n[grey]  AutoTaller Manager — sesión finalizada.[/]\n");
        await Task.Delay(800);
        break;
    }

    // Sesión activa — mostrar menú principal
    var mainMenu = new MainMenu(api, authResult);
    await mainMenu.ShowAsync();

    // Al salir del menú principal el token ya fue limpiado en MainMenu
    // Mostramos mensaje y volvemos al login
    AnsiConsole.Clear();
    AnsiConsole.Write(new Rule("[grey]Sesión cerrada[/]").RuleStyle("grey"));
    AnsiConsole.MarkupLine(
        $"[grey]  Hasta luego, [cyan]{Markup.Escape(authResult.Nombres)}[/]. Volviendo al inicio...[/]");
    await Task.Delay(1200);
}
