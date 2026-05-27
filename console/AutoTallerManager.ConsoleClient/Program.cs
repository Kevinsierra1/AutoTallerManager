using Microsoft.Extensions.Configuration;
using Spectre.Console;
using AutoTallerManager.ConsoleClient.Menus;
using AutoTallerManager.ConsoleClient.Services;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "http://localhost:5000/";

var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
var apiService = new ApiService(httpClient);
var authMenu = new AuthMenu(apiService);

// Login
AnsiConsole.Clear();
bool loggedIn = false;
int intentos = 0;
while (!loggedIn && intentos < 3)
{
    loggedIn = await authMenu.LoginAsync();
    if (!loggedIn)
    {
        intentos++;
        if (intentos < 3)
        {
            AnsiConsole.MarkupLine($"[red]Intento {intentos}/3. Intente de nuevo.[/]");
            await Task.Delay(1000);
        }
    }
}

if (!loggedIn)
{
    AnsiConsole.MarkupLine("[red]Número máximo de intentos alcanzado. Saliendo...[/]");
    return;
}

// Menus
var dashboardMenu = new DashboardMenu(apiService);
var clientesMenu = new ClientesMenu(apiService);
var ordenesMenu = new OrdenesMenu(apiService);
var inventarioMenu = new InventarioMenu(apiService);
var facturacionMenu = new FacturacionMenu(apiService);

// Main loop
while (true)
{
    AnsiConsole.Clear();
    AnsiConsole.Write(new FigletText("AutoTaller").Color(Color.Blue).Centered());
    AnsiConsole.Write(new Rule("[grey]Sistema de Gestión de Talleres Automotrices[/]").Centered());
    AnsiConsole.WriteLine();

    var opcion = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold]Menú Principal[/]")
            .PageSize(12)
            .AddChoices(
                "1. Clientes",
                "2. Vehículos",
                "3. Citas",
                "4. Órdenes",
                "5. Inventario",
                "6. Facturación",
                "7. Auditoría",
                "8. Dashboard",
                "9. Salir"
            )
    );

    switch (opcion[0])
    {
        case '1': await clientesMenu.ShowAsync(); break;
        case '4': await ordenesMenu.ShowAsync(); break;
        case '5': await inventarioMenu.ShowAsync(); break;
        case '6': await facturacionMenu.ShowAsync(); break;
        case '8': await dashboardMenu.ShowAsync(); break;
        case '9':
            if (AnsiConsole.Confirm("[yellow]¿Desea salir del sistema?[/]"))
            {
                AnsiConsole.MarkupLine("[grey]¡Hasta luego![/]");
                return;
            }
            break;
        default:
            AnsiConsole.MarkupLine("[yellow]Módulo en construcción...[/]");
            await Task.Delay(1000);
            break;
    }
}
