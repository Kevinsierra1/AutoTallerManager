using Spectre.Console;
using AutoTallerManager.ConsoleClient.Services;

namespace AutoTallerManager.ConsoleClient.Menus;

public class OrdenesMenu
{
    private readonly ApiService _api;

    public OrdenesMenu(ApiService api) => _api = api;

    public async Task ShowAsync()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold yellow]Gestión de Órdenes[/]").RuleStyle("yellow"));

            var opcion = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Seleccione una opción:")
                .AddChoices("Ver todas las órdenes", "Volver"));

            if (opcion == "Volver") break;
            if (opcion == "Ver todas las órdenes") await ListarOrdenesAsync();
        }
    }

    private async Task ListarOrdenesAsync()
    {
        await AnsiConsole.Status().StartAsync("Cargando órdenes...", async ctx =>
        {
            ctx.Spinner(Spinner.Known.BouncingBall);
            var ordenes = await _api.GetOrdenesAsync();
            AnsiConsole.WriteLine();

            if (ordenes == null || !ordenes.Items.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No hay órdenes registradas.[/]");
            }
            else
            {
                var table = new Table().BorderColor(Color.Yellow).Expand();
                table.AddColumn("N° Orden").AddColumn("Estado").AddColumn("Cliente").AddColumn("Vehículo").AddColumn("Fecha Ingreso");
                foreach (var o in ordenes.Items)
                {
                    var estadoColor = o.Estado switch
                    {
                        "Pendiente" => "yellow",
                        "EnProceso" => "blue",
                        "Finalizada" => "green",
                        "Cancelada" => "red",
                        _ => "white"
                    };
                    table.AddRow(
                        o.NumeroOrden,
                        $"[{estadoColor}]{o.Estado}[/]",
                        o.ClienteNombre ?? "-",
                        o.VehiculoPlaca ?? "-",
                        o.FechaIngreso.ToString("dd/MM/yyyy HH:mm")
                    );
                }
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"[grey]Total: {ordenes.TotalCount} órdenes[/]");
            }
        });
        AnsiConsole.Ask<string>("[grey]Presione Enter para continuar...[/]");
    }
}
