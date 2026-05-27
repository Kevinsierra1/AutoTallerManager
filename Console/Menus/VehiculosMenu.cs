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

        var placa = AskRequired("Placa (ej: ABC123):").ToUpper();

        // ── Seleccionar Marca ─────────────────────────────────────────────────
        List<CatalogoItem>? marcas = null;
        await WithSpinner("Cargando marcas", async () => { marcas = await Api.GetMarcasAsync(); });

        if (marcas == null || marcas.Count == 0) { Error("No hay marcas registradas en el sistema."); Pause(); return; }

        var marcaSel = AnsiConsole.Prompt(
            new SelectionPrompt<CatalogoItem>()
                .Title("[cyan]  Selecciona la marca:[/]")
                .PageSize(12)
                .HighlightStyle(new Style(Color.Cyan1))
                .UseConverter(m => m.Nombre)
                .AddChoices(marcas));

        // ── Seleccionar Modelo (filtrado por marca) ────────────────────────────
        List<ModeloItem>? modelos = null;
        await WithSpinner($"Cargando modelos de {marcaSel.Nombre}", async () =>
        {
            modelos = await Api.GetModelosAsync(marcaSel.Id);
        });

        if (modelos == null || modelos.Count == 0) { Error($"No hay modelos para la marca {marcaSel.Nombre}."); Pause(); return; }

        var modeloSel = AnsiConsole.Prompt(
            new SelectionPrompt<ModeloItem>()
                .Title("[cyan]  Selecciona el modelo:[/]")
                .PageSize(12)
                .HighlightStyle(new Style(Color.Cyan1))
                .UseConverter(m => m.Nombre)
                .AddChoices(modelos));

        // ── Seleccionar Color (opcional) ──────────────────────────────────────
        List<ColorItem>? colores = null;
        await WithSpinner("Cargando colores", async () => { colores = await Api.GetColoresAsync(); });

        Guid? colorId = null;
        if (colores != null && colores.Count > 0)
        {
            var opcionesColor = colores.Select(c => c.Nombre).ToList();
            opcionesColor.Insert(0, "Sin color / No especificar");

            var colorSel = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[cyan]  Selecciona el color:[/]")
                    .PageSize(14)
                    .HighlightStyle(new Style(Color.Cyan1))
                    .AddChoices(opcionesColor));

            if (colorSel != "Sin color / No especificar")
                colorId = colores.First(c => c.Nombre == colorSel).Id;
        }

        // ── Datos adicionales ─────────────────────────────────────────────────
        var anio = AskInt("Año:", DateTime.Now.Year);
        var vin  = Ask("VIN (opcional):");
        var obs  = Ask("Observaciones (opcional):");

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[grey]  Resumen:[/] [bold]{Markup.Escape(placa)}[/] — {Markup.Escape(marcaSel.Nombre)} {Markup.Escape(modeloSel.Nombre)} {anio}");
        if (!Confirm("¿Registrar este vehículo?")) return;

        VehiculoModel? created = null;
        await WithSpinner("Registrando vehículo", async () =>
        {
            created = await Api.CreateVehiculoAsync(new
            {
                Placa = placa,
                Vin = string.IsNullOrEmpty(vin) ? null : vin,
                ModeloVehiculoId = modeloSel.Id,
                ColorId = colorId,
                Anio = anio,
                Observaciones = string.IsNullOrEmpty(obs) ? null : obs
            });
        });

        if (created != null)
        {
            Ok($"Vehículo {Markup.Escape(created.Placa)} registrado correctamente.");
            AnsiConsole.MarkupLine($"[grey]  Marca/Modelo:[/] {Markup.Escape(created.MarcaModelo)}  [grey]  ID:[/] [dim]{created.Id}[/]");
        }
        else
            Error("No se pudo registrar el vehículo.");

        Pause();
    }
}
