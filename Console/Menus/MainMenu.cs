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

            // ── Header ──────────────────────────────────────────────────────
            AnsiConsole.Write(new Rule($"[bold blue]  AutoTaller Manager[/]").RuleStyle("blue"));

            var rolColor = User.EsAdmin() ? "red" :
                           User.EsJefeTaller() ? "yellow" :
                           User.EsMecanico() ? "cyan" :
                           User.EsCliente() ? "green" :
                           User.EsAlmacen() ? "magenta" : "white";

            AnsiConsole.MarkupLine(
                $"  [grey]Bienvenido,[/] [bold cyan]{Markup.Escape(User.NombreCompleto)}[/]" +
                $"   [grey]Rol:[/] [{rolColor}]{Markup.Escape(User.RolesStr)}[/]");
            AnsiConsole.WriteLine();

            // ── Menú según rol ───────────────────────────────────────────────
            var opciones = BuildOpciones();

            var opcion = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]  Menú Principal[/]")
                    .PageSize(14)
                    .HighlightStyle(new Style(Color.Cyan1))
                    .AddChoices(opciones));

            if (opcion.Contains("Cerrar Sesión"))
            {
                if (AnsiConsole.Confirm("  ¿Deseas cerrar sesión?", false))
                {
                    Api.ClearToken();
                    return;
                }
                continue;
            }

            await RoutearOpcion(opcion);
        }
    }

    private List<string> BuildOpciones()
    {
        var lista = new List<string>();

        // Dashboard — Admin, JefeTaller, Mecánico, Almacén
        if (User.EsAdmin() || User.EsJefeTaller() || User.EsMecanico() || User.EsAlmacen())
            lista.Add("  [cyan]📊[/] Dashboard");

        // Clientes — Admin, JefeTaller, Recepcionista
        if (User.EsAdmin() || User.EsJefeTaller() || User.EsRecepcionista())
            lista.Add("  [cyan]👥[/] Clientes");

        // Vehículos — Admin, JefeTaller, Recepcionista
        if (User.EsAdmin() || User.EsJefeTaller() || User.EsRecepcionista())
            lista.Add("  [cyan]🚗[/] Vehículos");

        // Órdenes — Solo personal del taller (NO el cliente)
        // El cliente no crea órdenes, las crea la Recepcionista
        if (User.EsAdmin() || User.EsJefeTaller() || User.EsRecepcionista() || User.EsMecanico())
            lista.Add("  [cyan]🔧[/] Órdenes de Servicio");

        // Mini-Órdenes — Mecánico, JefeTaller, Cliente (solo aprobación), Admin
        if (User.PuedeGestionarMiniOrdenes())
            lista.Add("  [cyan]📋[/] Mini-Órdenes");

        // Inventario — Admin, JefeTaller, Almacén
        if (User.PuedeVerInventario())
            lista.Add("  [cyan]📦[/] Inventario & Repuestos");

        // Facturación — Admin, JefeTaller, Recepcionista
        if (User.EsAdmin() || User.EsJefeTaller() || User.EsRecepcionista())
            lista.Add("  [cyan]💰[/] Facturación");

        // Administración — solo Admin
        if (User.EsAdmin())
            lista.Add("  [red]⚙[/]  Administración");

        lista.Add("  [red]🚪[/] Cerrar Sesión");
        return lista;
    }

    private async Task RoutearOpcion(string opcion)
    {
        if (opcion.Contains("Dashboard"))
            await new DashboardMenu(Api, User).ShowAsync();
        else if (opcion.Contains("Clientes"))
            await new ClientesMenu(Api, User).ShowAsync();
        else if (opcion.Contains("Vehículos"))
            await new VehiculosMenu(Api, User).ShowAsync();
        else if (opcion.Contains("Órdenes de Servicio"))
            await new OrdenesMenu(Api, User).ShowAsync();
        else if (opcion.Contains("Mini-Órdenes"))
            await new MiniOrdenesMenu(Api, User).ShowAsync();
        else if (opcion.Contains("Inventario"))
            await new InventarioMenu(Api, User).ShowAsync();
        else if (opcion.Contains("Facturación"))
            await new FacturacionMenu(Api, User).ShowAsync();
        else if (opcion.Contains("Administración"))
            await new AdminMenu(Api, User).ShowAsync();
    }
}
