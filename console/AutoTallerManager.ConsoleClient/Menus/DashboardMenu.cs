using Spectre.Console;
using AutoTallerManager.ConsoleClient.Services;

namespace AutoTallerManager.ConsoleClient.Menus;

public class DashboardMenu
{
    private readonly ApiService _api;

    public DashboardMenu(ApiService api) => _api = api;

    public async Task ShowAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold blue]Dashboard — Resumen Ejecutivo[/]").RuleStyle("blue"));

        await AnsiConsole.Status().StartAsync("Cargando datos...", async ctx =>
        {
            ctx.Spinner(Spinner.Known.BouncingBar);
            var resumen = await _api.GetDashboardAsync();
            var repuestos = await _api.GetRepuestosAsync(critico: true);

            AnsiConsole.WriteLine();

            // KPI Grid
            var grid = new Grid();
            grid.AddColumn(); grid.AddColumn(); grid.AddColumn();
            grid.AddRow(
                new Panel($"[bold]{resumen?.TotalClientes ?? 0}[/]
[grey]Clientes[/]").BorderColor(Color.Blue).Expand(),
                new Panel($"[bold]{resumen?.TotalVehiculos ?? 0}[/]
[grey]Vehículos[/]").BorderColor(Color.Cyan1).Expand(),
                new Panel($"[bold]{resumen?.OrdenesActivas ?? 0}[/]
[grey]Órdenes Activas[/]").BorderColor(Color.Orange1).Expand()
            );
            grid.AddRow(
                new Panel($"[bold]{resumen?.OrdenesFinalizadas ?? 0}[/]
[grey]Órdenes Finalizadas[/]").BorderColor(Color.Green).Expand(),
                new Panel($"[bold red]{resumen?.RepuestosCriticos ?? 0}[/]
[grey]Repuestos Críticos[/]").BorderColor(Color.Red).Expand(),
                new Panel($"[bold green]${resumen?.FacturacionMensual:N2}[/]
[grey]Facturación Mensual[/]").BorderColor(Color.Gold1).Expand()
            );
            AnsiConsole.Write(grid);

            // Repuestos críticos
            if (repuestos?.Items?.Any() == true)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Rule("[red]⚠ Repuestos con Stock Crítico[/]").RuleStyle("red"));
                var table = new Table().BorderColor(Color.Red).Expand();
                table.AddColumn("Código").AddColumn("Nombre").AddColumn("[red]Stock Actual[/]").AddColumn("Stock Mínimo");
                foreach (var r in repuestos.Items)
                    table.AddRow(r.Codigo, r.Nombre, $"[red]{r.StockActual}[/]", r.StockMinimo.ToString());
                AnsiConsole.Write(table);
            }
        });

        AnsiConsole.WriteLine();
        AnsiConsole.Ask<string>("[grey]Presione Enter para continuar...[/]");
    }
}
