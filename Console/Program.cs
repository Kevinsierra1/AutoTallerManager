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

// ── Login ─────────────────────────────────────────────────────────────────────

var authMenu = new AuthMenu(api);
var authResult = await authMenu.ShowAsync();

if (authResult == null)
{
    AnsiConsole.Clear();
    AnsiConsole.MarkupLine("[grey]¡Hasta luego![/]");
    await Task.Delay(800);
    return;
}

// ── Menú Principal ────────────────────────────────────────────────────────────

var mainMenu = new MainMenu(api, authResult);
await mainMenu.ShowAsync();

AnsiConsole.Clear();
AnsiConsole.Write(new Rule("[grey]Sesión finalizada[/]").RuleStyle("grey"));
AnsiConsole.MarkupLine($"\n[grey]  Hasta luego, [cyan]{authResult.Nombres}[/]![/]\n");
