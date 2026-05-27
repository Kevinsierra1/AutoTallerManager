using Spectre.Console;
using AutoTallerManager.ConsoleClient.Services;

namespace AutoTallerManager.ConsoleClient.Menus;

public class InventarioMenu
{
    private readonly ApiService _api;

    public InventarioMenu(ApiService api) => _api = api;

    public async Task ShowAsync()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold green]Gestión de Inventario[/]").RuleStyle("green"));

            var opcion = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Seleccione una opción:")
                .AddChoices("Ver repuestos", "Ver repuestos críticos", "Volver"));

            if (opcion == "Volver") break;

            bool critico = opcion == "Ver repuestos críticos";
            await ListarRepuestosAsync(critico);
        }
    }

    private async Task ListarRepuestosAsync(bool critico)
    {
        await AnsiConsole.Status().StartAsync("Cargando repuestos...", async ctx =>
        {
            ctx.Spinner(Spinner.Known.Dots);
            var repuestos = await _api.GetRepuestosAsync(critico: critico ? true : null);
            AnsiConsole.WriteLine();

            if (repuestos == null || !repuestos.Items.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No hay repuestos.[/]");
            }
            else
            {
                var table = new Table().BorderColor(critico ? Color.Red : Color.Green).Expand();
                table.AddColumn("Código").AddColumn("Nombre").AddColumn("Stock").AddColumn("Mínimo").AddColumn("Precio Venta");
                foreach (var r in repuestos.Items)
                {
                    var stockColor = r.StockActual <= r.StockMinimo ? "red" : "green";
                    table.AddRow(r.Codigo, r.Nombre, $"[{stockColor}]{r.StockActual}[/]", r.StockMinimo.ToString(), $"${r.PrecioVenta:N2}");
                }
                AnsiConsole.Write(table);
            }
        });
        AnsiConsole.Ask<string>("[grey]Presione Enter para continuar...[/]");
    }
}
