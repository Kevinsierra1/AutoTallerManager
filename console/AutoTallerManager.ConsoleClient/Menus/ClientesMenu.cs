using Spectre.Console;
using AutoTallerManager.ConsoleClient.Services;

namespace AutoTallerManager.ConsoleClient.Menus;

public class ClientesMenu
{
    private readonly ApiService _api;

    public ClientesMenu(ApiService api) => _api = api;

    public async Task ShowAsync()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold cyan]Gestión de Clientes[/]").RuleStyle("cyan"));

            var opcion = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Seleccione una opción:")
                .AddChoices("Ver todos los clientes", "Volver"));

            if (opcion == "Volver") break;

            if (opcion == "Ver todos los clientes")
                await ListarClientesAsync();
        }
    }

    private async Task ListarClientesAsync()
    {
        await AnsiConsole.Status().StartAsync("Cargando clientes...", async ctx =>
        {
            ctx.Spinner(Spinner.Known.Dots);
            var clientes = await _api.GetClientesAsync();
            AnsiConsole.WriteLine();

            if (clientes == null || !clientes.Items.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No hay clientes registrados.[/]");
            }
            else
            {
                var table = new Table().BorderColor(Color.Cyan1).Expand();
                table.AddColumn("#").AddColumn("Nombres").AddColumn("Apellidos").AddColumn("Documento").AddColumn("Teléfono");
                int i = 1;
                foreach (var c in clientes.Items)
                    table.AddRow(i++.ToString(), c.Nombres, c.Apellidos, c.NumeroDocumento, c.Telefono ?? "-");
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"[grey]Total: {clientes.TotalCount} clientes[/]");
            }
        });

        AnsiConsole.Ask<string>("[grey]Presione Enter para continuar...[/]");
    }
}
