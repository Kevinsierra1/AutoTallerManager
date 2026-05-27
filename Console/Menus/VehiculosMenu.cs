using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public class VehiculosMenu : BaseMenu
{
    public VehiculosMenu(ApiService api, AuthResponse user) : base(api, user) { }

    public async Task ShowAsync()
    {
        while (true)
        {
            PrintHeader("Gestión de Vehículos");

            var opcion = Choice("  Vehículos:",
                "  Listar Vehículos",
                "  Buscar por Placa",
                "  Registrar Vehículo",
                "  Volver al Menú Principal");

            if (opcion.Contains("Volver")) return;

            if (opcion.Contains("Listar"))   await ListarAsync();
            else if (opcion.Contains("Buscar"))   await BuscarAsync();
            else if (opcion.Contains("Registrar")) await RegistrarAsync();
        }
    }

    private async Task ListarAsync(string? placa = null)
    {
        PrintHeader("Vehículos", placa != null ? $"Placa: \"{placa}\"" : "Todos los vehículos");

        PagedData<VehiculoModel>? data = null;
        await WithSpinner("Cargando vehículos", async () =>
        {
            data = await Api.GetVehiculosAsync(1, 20, placa);
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData(placa != null ? "No se encontró ningún vehículo con esa placa." : "No hay vehículos registrados.");
            Pause();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .Expand();

        table.AddColumn(new TableColumn("[bold]Placa[/]"));
        table.AddColumn(new TableColumn("[bold]Marca / Modelo[/]"));
        table.AddColumn(new TableColumn("[bold]Color[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Año[/]").Centered());
        table.AddColumn(new TableColumn("[bold]KM[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Estado[/]").Centered());

        foreach (var v in data.Items)
        {
            var estado = v.Activo ? "[green]Activo[/]" : "[red]Inactivo[/]";
            table.AddRow(
                $"[bold]{Markup.Escape(v.Placa)}[/]",
                Markup.Escape(v.MarcaModelo),
                Markup.Escape(v.Color ?? "-"),
                v.Anio.ToString(),
                $"{v.KilometrajeActual:N0} km",
                estado
            );
        }

        AnsiConsole.Write(table);
        Info($"Mostrando {data.Items.Count} de {data.TotalCount} vehículos");
        Pause();
    }

    private async Task BuscarAsync()
    {
        PrintHeader("Buscar Vehículo");
        var placa = AskRequired("Ingresa la placa (ej: ABC123):");
        await ListarAsync(placa.ToUpper());
    }

    private async Task RegistrarAsync()
    {
        PrintHeader("Registrar Vehículo");

        // Modelos de vehículo del seed (fijos)
        var marcasModelos = new Dictionary<string, (string marcaNombre, List<(string id, string nombre)> modelos)>
        {
            // Usaremos GUIDs conocidos del seed o dejaremos al usuario escribirlos
            // Como no hay endpoint de catálogos, mostramos una selección básica
        };

        var placa = AskRequired("Placa (ej: ABC123):").ToUpper();

        AnsiConsole.WriteLine();
        Info("Nota: Para ModeloVehiculoId y ColorId usa los GUIDs de la base de datos.");
        Info("Puedes consultarlos en Swagger: GET /api/Vehiculos y ver los existentes.");
        AnsiConsole.WriteLine();

        var modeloIdStr = AskRequired("ModeloVehiculoId (GUID):");
        if (!Guid.TryParse(modeloIdStr, out var modeloId)) { Error("GUID inválido."); Pause(); return; }

        var colorIdStr = Ask("ColorId (GUID, opcional):");
        Guid? colorId = null;
        if (!string.IsNullOrEmpty(colorIdStr) && Guid.TryParse(colorIdStr, out var cid)) colorId = cid;

        var anio = AskInt("Año:", DateTime.Now.Year);
        var vin  = Ask("VIN (opcional):");
        var obs  = Ask("Observaciones (opcional):");

        AnsiConsole.WriteLine();
        if (!Confirm("¿Registrar este vehículo?")) return;

        VehiculoModel? created = null;
        await WithSpinner("Registrando vehículo", async () =>
        {
            created = await Api.CreateVehiculoAsync(new
            {
                Placa = placa,
                Vin = string.IsNullOrEmpty(vin) ? null : vin,
                ModeloVehiculoId = modeloId,
                ColorId = colorId,
                Anio = anio,
                Observaciones = string.IsNullOrEmpty(obs) ? null : obs
            });
        });

        if (created != null)
            Ok($"Vehículo [bold]{created.Placa}[/] registrado correctamente.");
        else
            Error("No se pudo registrar el vehículo. Verifica los IDs ingresados.");

        Pause();
    }
}
