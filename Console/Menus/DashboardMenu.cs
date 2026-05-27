using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public class DashboardMenu : BaseMenu
{
    public DashboardMenu(ApiService api, AuthResponse user) : base(api, user) { }

    public async Task ShowAsync()
    {
        PrintHeader("Dashboard", "Resumen ejecutivo del taller");

        DashboardResumen? resumen = null;
        List<OrdenEstadistica>? ordenes = null;
        List<FacturacionMensual>? facturacion = null;

        await WithSpinner("Cargando dashboard", async () =>
        {
            resumen = await Api.GetDashboardResumenAsync();
            ordenes = await Api.GetDashboardOrdenesAsync();
            try { facturacion = await Api.GetDashboardFacturacionAsync(); } catch { }
        });

        if (resumen == null)
        {
            Error("No se pudo cargar el dashboard.");
            Pause();
            return;
        }

        // ── KPI Cards ──────────────────────────────────────────────────────
        var kpiTable = new Table().BorderStyle(Style.Parse("grey")).Expand();
        kpiTable.AddColumn(new TableColumn("[bold]Clientes[/]").Centered());
        kpiTable.AddColumn(new TableColumn("[bold]Vehículos[/]").Centered());
        kpiTable.AddColumn(new TableColumn("[bold]Órdenes Activas[/]").Centered());
        kpiTable.AddColumn(new TableColumn("[bold]Finalizadas[/]").Centered());
        kpiTable.AddColumn(new TableColumn("[bold]Repuestos Críticos[/]").Centered());
        kpiTable.AddColumn(new TableColumn("[bold]Facturación Mes[/]").Centered());

        var criticoColor = resumen.RepuestosCriticos > 0 ? "red" : "green";

        kpiTable.AddRow(
            $"[cyan]{resumen.TotalClientes}[/]",
            $"[cyan]{resumen.TotalVehiculos}[/]",
            $"[yellow]{resumen.OrdenesActivas}[/]",
            $"[green]{resumen.OrdenesFinalizadas}[/]",
            $"[{criticoColor}]{resumen.RepuestosCriticos}[/]",
            $"[green]$ {resumen.FacturacionMensual:N2}[/]"
        );

        AnsiConsole.Write(new Panel(kpiTable)
            .Header("[bold blue]  KPIs del Taller[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("blue")));

        AnsiConsole.WriteLine();

        // ── Órdenes por estado ─────────────────────────────────────────────
        if (ordenes?.Count > 0)
        {
            var chart = new BarChart()
                .Width(60)
                .Label("[bold grey]Órdenes por Estado[/]")
                .CenterLabel();

            foreach (var o in ordenes)
            {
                var color = o.Estado switch
                {
                    "Pendiente"  => Color.Yellow,
                    "Aprobada"   => Color.Blue,
                    "EnProceso"  => Color.Cyan1,
                    "Finalizada" => Color.Green,
                    "Cancelada"  => Color.Red,
                    _ => Color.White
                };
                chart.AddItem(o.Estado, o.Total, color);
            }

            AnsiConsole.Write(new Panel(chart)
                .Header("[bold blue]  Distribución de Órdenes[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("blue")));

            AnsiConsole.WriteLine();
        }

        // ── Facturación mensual ────────────────────────────────────────────
        if (facturacion?.Count > 0)
        {
            var factChart = new BarChart()
                .Width(60)
                .Label("[bold grey]Facturación Últimos 6 Meses (miles $)[/]")
                .CenterLabel();

            foreach (var f in facturacion)
                factChart.AddItem(f.Mes, (double)(f.Total / 1000m), Color.Green);

            AnsiConsole.Write(new Panel(factChart)
                .Header("[bold blue]  Facturación Mensual[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("blue")));

            AnsiConsole.WriteLine();
        }

        if (resumen.RepuestosCriticos > 0)
        {
            Warn($"Hay {resumen.RepuestosCriticos} repuesto(s) con stock crítico. Revisa el módulo de Inventario.");
            AnsiConsole.WriteLine();
        }

        Pause();
    }
}
