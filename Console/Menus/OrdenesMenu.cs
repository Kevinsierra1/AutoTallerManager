using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public class OrdenesMenu : BaseMenu
{
    public OrdenesMenu(ApiService api, AuthResponse user) : base(api, user) { }

    public async Task ShowAsync()
    {
        while (true)
        {
            PrintHeader("Órdenes de Servicio");

            var opcion = Choice("  Órdenes:",
                "  Listar Órdenes",
                "  Filtrar por Estado",
                "  Ver Detalle de Orden",
                "  Crear Orden",
                "  Aprobar Orden",
                "  Asignar Mecánico",
                "  Finalizar Orden",
                "  Cancelar Orden",
                "  Volver al Menú Principal");

            if (opcion.Contains("Volver")) return;

            if (opcion.Contains("Listar"))       await ListarAsync();
            else if (opcion.Contains("Filtrar"))      await FiltrarPorEstadoAsync();
            else if (opcion.Contains("Detalle"))      await VerDetalleAsync();
            else if (opcion.Contains("Crear"))        await CrearAsync();
            else if (opcion.Contains("Aprobar"))      await AprobarAsync();
            else if (opcion.Contains("Asignar"))      await AsignarMecanicoAsync();
            else if (opcion.Contains("Finalizar"))    await FinalizarAsync();
            else if (opcion.Contains("Cancelar"))     await CancelarAsync();
        }
    }

    // ── Tabla de órdenes ────────────────────────────────────────────────────

    private static void MostrarTablaOrdenes(IEnumerable<OrdenModel> items)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .Expand();

        table.AddColumn(new TableColumn("[bold]# Orden[/]"));
        table.AddColumn(new TableColumn("[bold]Cliente[/]"));
        table.AddColumn(new TableColumn("[bold]Placa[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Mecánico[/]"));
        table.AddColumn(new TableColumn("[bold]Estado[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Ingreso[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Total[/]").RightAligned());

        foreach (var o in items)
        {
            table.AddRow(
                $"[bold]{Markup.Escape(o.NumeroOrden)}[/]",
                Markup.Escape(o.ClienteNombre ?? "-"),
                Markup.Escape(o.VehiculoPlaca ?? "-"),
                Markup.Escape(o.MecanicoNombre ?? "[grey]Sin asignar[/]"),
                $"[{o.EstadoColor}]{Markup.Escape(o.EstadoTexto)}[/]",
                o.FechaIngreso.ToString("dd/MM/yyyy"),
                o.Total.HasValue ? $"$ {o.Total:N2}" : "[grey]-[/]"
            );
        }

        AnsiConsole.Write(table);
    }

    // ── Listar ──────────────────────────────────────────────────────────────

    private async Task ListarAsync(int? estado = null)
    {
        var titulo = estado.HasValue
            ? $"Estado: {(EstadoOrden)estado}" : "Todas las órdenes";
        PrintHeader("Órdenes de Servicio", titulo);

        PagedData<OrdenModel>? data = null;
        await WithSpinner("Cargando órdenes", async () =>
        {
            data = await Api.GetOrdenesAsync(1, 20, estado);
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData("No se encontraron órdenes.");
            Pause();
            return;
        }

        MostrarTablaOrdenes(data.Items);
        Info($"Mostrando {data.Items.Count} de {data.TotalCount} órdenes");
        Pause();
    }

    // ── Filtrar por Estado ──────────────────────────────────────────────────

    private async Task FiltrarPorEstadoAsync()
    {
        PrintHeader("Filtrar por Estado");

        var estadoStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Selecciona el estado:[/]")
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices("Pendiente", "Aprobada", "EnProceso", "Finalizada", "Cancelada", "Todos"));

        int? estado = estadoStr switch
        {
            "Pendiente"  => 0,
            "Aprobada"   => 1,
            "EnProceso"  => 2,
            "Finalizada" => 3,
            "Cancelada"  => 4,
            _ => null
        };

        await ListarAsync(estado);
    }

    // ── Ver Detalle ─────────────────────────────────────────────────────────

    private async Task VerDetalleAsync()
    {
        PrintHeader("Detalle de Orden");

        var numero = AskRequired("Número de orden o ID (GUID):");
        OrdenModel? orden = null;

        await WithSpinner("Buscando orden", async () =>
        {
            if (Guid.TryParse(numero, out var id))
                orden = await Api.GetOrdenByIdAsync(id);
            else
            {
                var lista = await Api.GetOrdenesAsync(1, 100);
                orden = lista?.Items.FirstOrDefault(o =>
                    o.NumeroOrden.Contains(numero, StringComparison.OrdinalIgnoreCase));
            }
        });

        if (orden == null) { NoData("Orden no encontrada."); Pause(); return; }

        var grid = new Grid();
        grid.AddColumn(); grid.AddColumn();

        void Row(string label, string value) =>
            grid.AddRow($"[grey]  {label}:[/]", $"[white]{value}[/]");

        Row("Número", orden.NumeroOrden);
        Row("Estado", $"[{orden.EstadoColor}]{orden.EstadoTexto}[/]");
        Row("Cliente", orden.ClienteNombre ?? "-");
        Row("Vehículo", orden.VehiculoPlaca ?? "-");
        Row("Mecánico", orden.MecanicoNombre ?? "Sin asignar");
        Row("Descripción", orden.Descripcion ?? "-");
        Row("Fecha Ingreso", orden.FechaIngreso.ToString("dd/MM/yyyy HH:mm"));
        if (orden.FechaFin.HasValue) Row("Fecha Fin", orden.FechaFin.Value.ToString("dd/MM/yyyy HH:mm"));
        if (orden.Total.HasValue) Row("Total", $"$ {orden.Total:N2}");

        AnsiConsole.Write(new Panel(grid)
            .Header($"[bold]  Orden #{Markup.Escape(orden.NumeroOrden)}[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("blue")));

        Pause();
    }

    // ── Crear ───────────────────────────────────────────────────────────────

    private async Task CrearAsync()
    {
        PrintHeader("Crear Orden de Servicio");

        var clienteIdStr = AskRequired("ClienteId (GUID):");
        if (!Guid.TryParse(clienteIdStr, out var clienteId)) { Error("GUID inválido."); Pause(); return; }

        var vehiculoIdStr = AskRequired("VehiculoId (GUID):");
        if (!Guid.TryParse(vehiculoIdStr, out var vehiculoId)) { Error("GUID inválido."); Pause(); return; }

        var desc = Ask("Descripción del trabajo (opcional):");

        Info("TipoServicioId es opcional. Déjalo vacío para omitir.");
        var tipoIdStr = Ask("TipoServicioId (GUID, opcional):");
        Guid? tipoId = null;
        if (!string.IsNullOrEmpty(tipoIdStr) && Guid.TryParse(tipoIdStr, out var tid)) tipoId = tid;

        AnsiConsole.WriteLine();
        if (!Confirm("¿Crear la orden?")) return;

        OrdenModel? created = null;
        await WithSpinner("Creando orden", async () =>
        {
            created = await Api.CreateOrdenAsync(new
            {
                ClienteId = clienteId,
                VehiculoId = vehiculoId,
                Descripcion = string.IsNullOrEmpty(desc) ? null : desc,
                TipoServicioId = tipoId
            });
        });

        if (created != null)
            Ok($"Orden [bold]{created.NumeroOrden}[/] creada en estado [yellow]Pendiente[/].");
        else
            Error("No se pudo crear la orden.");

        Pause();
    }

    // ── Aprobar ─────────────────────────────────────────────────────────────

    private async Task AprobarAsync()
    {
        PrintHeader("Aprobar Orden");

        var orden = await SeleccionarOrdenAsync(0); // Pendiente
        if (orden == null) return;

        var clienteIdStr = AskRequired($"ClienteId que aprueba (GUID) [{orden.ClienteId}]:");
        if (!Guid.TryParse(clienteIdStr, out var clienteId)) clienteId = orden.ClienteId;

        if (!Confirm($"¿Aprobar la orden {orden.NumeroOrden}?")) return;

        (bool ok, string? error) result = (false, null);
        await WithSpinner("Aprobando", async () => { result = await Api.AprobarOrdenAsync(orden.Id, clienteId); });

        if (result.ok) Ok("Orden aprobada correctamente.");
        else Error(result.error ?? "Error al aprobar.");
        Pause();
    }

    // ── Asignar Mecánico ────────────────────────────────────────────────────

    private async Task AsignarMecanicoAsync()
    {
        PrintHeader("Asignar Mecánico");

        var orden = await SeleccionarOrdenAsync();
        if (orden == null) return;

        // Cargar empleados
        PagedData<EmpleadoModel>? empleados = null;
        await WithSpinner("Cargando mecánicos", async () =>
        {
            empleados = await Api.GetEmpleadosAsync();
        });

        Guid empleadoId;

        if (empleados?.Items.Count > 0)
        {
            var opciones = empleados.Items.Select(e => $"{e.NombreCompleto} [{e.Id}]").ToList();
            opciones.Add("Ingresar ID manualmente");

            var sel = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[cyan]  Selecciona el mecánico:[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Cyan1))
                    .AddChoices(opciones));

            if (sel == "Ingresar ID manualmente")
            {
                var idStr = AskRequired("EmpleadoId (GUID):");
                if (!Guid.TryParse(idStr, out empleadoId)) { Error("GUID inválido."); Pause(); return; }
            }
            else
            {
                var emp = empleados.Items[opciones.IndexOf(sel)];
                empleadoId = emp.Id;
            }
        }
        else
        {
            var idStr = AskRequired("EmpleadoId (GUID):");
            if (!Guid.TryParse(idStr, out empleadoId)) { Error("GUID inválido."); Pause(); return; }
        }

        if (!Confirm($"¿Asignar mecánico a la orden {orden.NumeroOrden}?")) return;

        (bool ok, string? error) result = (false, null);
        await WithSpinner("Asignando", async () => { result = await Api.AsignarMecanicoAsync(orden.Id, empleadoId); });

        if (result.ok) Ok("Mecánico asignado correctamente.");
        else Error(result.error ?? "Error al asignar.");
        Pause();
    }

    // ── Finalizar ───────────────────────────────────────────────────────────

    private async Task FinalizarAsync()
    {
        PrintHeader("Finalizar Orden");

        var orden = await SeleccionarOrdenAsync(2); // EnProceso
        if (orden == null) return;

        if (!Confirm($"¿Finalizar la orden {orden.NumeroOrden}? Se marcará como completada.")) return;

        (bool ok, string? error) result = (false, null);
        await WithSpinner("Finalizando", async () => { result = await Api.FinalizarOrdenAsync(orden.Id); });

        if (result.ok) Ok("Orden finalizada correctamente. Ya puede generarse factura.");
        else Error(result.error ?? "Error al finalizar.");
        Pause();
    }

    // ── Cancelar ────────────────────────────────────────────────────────────

    private async Task CancelarAsync()
    {
        PrintHeader("Cancelar Orden");

        var orden = await SeleccionarOrdenAsync();
        if (orden == null) return;

        var motivo = AskRequired("Motivo de cancelación:");

        if (!Confirm($"¿Cancelar la orden {orden.NumeroOrden}?")) return;

        (bool ok, string? error) result = (false, null);
        await WithSpinner("Cancelando", async () => { result = await Api.CancelarOrdenAsync(orden.Id, motivo); });

        if (result.ok) Ok("Orden cancelada.");
        else Error(result.error ?? "Error al cancelar.");
        Pause();
    }

    // ── Helper: Seleccionar orden de una lista ───────────────────────────────

    private async Task<OrdenModel?> SeleccionarOrdenAsync(int? estadoFiltro = null)
    {
        PagedData<OrdenModel>? data = null;
        await WithSpinner("Cargando órdenes", async () =>
        {
            data = await Api.GetOrdenesAsync(1, 50, estadoFiltro);
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData("No se encontraron órdenes.");
            Pause();
            return null;
        }

        var opciones = data.Items
            .Select(o => $"[{o.EstadoColor}]{o.NumeroOrden}[/]  {o.ClienteNombre ?? "-"}  |  {o.VehiculoPlaca ?? "-"}  |  {o.EstadoTexto}")
            .ToList();
        opciones.Add("Cancelar");

        var sel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Selecciona la orden:[/]")
                .PageSize(12)
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices(opciones));

        if (sel == "Cancelar") return null;
        return data.Items[opciones.IndexOf(sel)];
    }
}
