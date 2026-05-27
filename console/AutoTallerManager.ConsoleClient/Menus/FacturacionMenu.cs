using Spectre.Console;
using AutoTallerManager.ConsoleClient.Services;

namespace AutoTallerManager.ConsoleClient.Menus;

public class FacturacionMenu
{
    private readonly ApiService _api;

    public FacturacionMenu(ApiService api) => _api = api;

    public async Task ShowAsync()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold gold1]Facturación[/]").RuleStyle("gold1"));

            var opcion = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Seleccione una opción:")
                .AddChoices("Ver facturas", "Volver"));

            if (opcion == "Volver") break;
            if (opcion == "Ver facturas") await ListarFacturasAsync();
        }
    }

    private async Task ListarFacturasAsync()
    {
        await AnsiConsole.Status().StartAsync("Cargando facturas...", async ctx =>
        {
            ctx.Spinner(Spinner.Known.Dots);
            var facturas = await _api.GetFacturasAsync();
            AnsiConsole.WriteLine();

            if (facturas == null || !facturas.Items.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No hay facturas registradas.[/]");
            }
            else
            {
                var table = new Table().BorderColor(Color.Gold1).Expand();
                table.AddColumn("N° Factura").AddColumn("Cliente").AddColumn("Total").AddColumn("Fecha");
                foreach (var f in facturas.Items)
                    table.AddRow(f.NumeroFactura, f.ClienteNombre ?? "-", $"[green]${f.Total:N2}[/]", f.FechaEmision.ToString("dd/MM/yyyy"));
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"[grey]Total: {facturas.TotalCount} facturas[/]");
            }
        });
        AnsiConsole.Ask<string>("[grey]Presione Enter para continuar...[/]");
    }
}
