using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public class MainMenu : BaseMenu
{
    public MainMenu(ApiService api, AuthResponse user) : base(api, user) { }

    public async Task ShowAsync()
    {
        while (true)
        {
            try { AnsiConsole.Clear(); } catch { }
            AnsiConsole.Write(new Rule($"[bold blue]  AutoTaller Manager[/]").RuleStyle("blue"));
            AnsiConsole.MarkupLine($"  [grey]Usuario:[/] [bold cyan]{Markup.Escape(User.NombreCompleto)}[/]   [grey]Rol:[/] [yellow]{Markup.Escape(User.RolesStr)}[/]");
            AnsiConsole.WriteLine();

            var opcion = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]  Menu Principal[/]")
                    .PageSize(12)
                    .HighlightStyle(new Style(Color.Cyan1))
                    .AddChoices(
                        "  [cyan]📊[/] Dashboard",
                        "  [cyan]👥[/] Clientes",
                        "  [cyan]🚗[/] Vehículos",
                        "  [cyan]🔧[/] Órdenes de Servicio",
                        "  [cyan]📦[/] Inventario & Repuestos",
                        "  [cyan]💰[/] Facturación",
                        "  [red]🚪[/] Cerrar Sesión"));

            if (opcion.Contains("Cerrar Sesión"))
            {
                if (AnsiConsole.Confirm("  ¿Deseas cerrar sesión?", false))
                {
                    Api.ClearToken();
                    return;
                }
                continue;
            }

            if (opcion.Contains("Dashboard"))
                await new DashboardMenu(Api, User).ShowAsync();
            else if (opcion.Contains("Clientes"))
                await new ClientesMenu(Api, User).ShowAsync();
            else if (opcion.Contains("Vehículos"))
                await new VehiculosMenu(Api, User).ShowAsync();
            else if (opcion.Contains("Órdenes"))
                await new OrdenesMenu(Api, User).ShowAsync();
            else if (opcion.Contains("Inventario"))
                await new InventarioMenu(Api, User).ShowAsync();
            else if (opcion.Contains("Facturación"))
                await new FacturacionMenu(Api, User).ShowAsync();
        }
    }
}
