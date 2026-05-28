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

            var opciones = BuildOpciones();
            var opcion = Choice("  Órdenes:", opciones.ToArray());

            if (opcion.Contains("Volver")) return;

            if (opcion.Contains("Listar"))         await ListarAsync();
            else if (opcion.Contains("Filtrar"))   await FiltrarPorEstadoAsync();
            else if (opcion.Contains("Detalle"))   await VerDetalleAsync();
            else if (opcion.Contains("Crear"))     await CrearAsync();
            else if (opcion.Contains("Aprobar"))   await AprobarAsync();
            else if (opcion.Contains("Mecánico"))  await AsignarMecanicoAsync();
            else if (opcion.Contains("Finalizar")) await FinalizarAsync();
            else if (opcion.Contains("Cancelar"))  await CancelarAsync();
        }
    }

    private List<string> BuildOpciones()
    {
        var lista = new List<string>
        {
            "  Listar Órdenes",
            "  Filtrar por Estado",
            "  Ver Detalle de Orden"
        };

        // Solo Recepcionista y JefeTaller crean órdenes
        if (User.EsRecepcionista() || User.EsJefeTaller())
            lista.Add("  Crear Orden");

        // Solo JefeTaller aprueba el inicio del trabajo
        if (User.EsJefeTaller())
            lista.Add("  Aprobar Orden");

        // Recepcionista y JefeTaller asignan mecánico
        if (User.EsRecepcionista() || User.EsJefeTaller())
            lista.Add("  Asignar Mecánico");

        // JefeTaller y Mecánico finalizan
        if (User.EsJefeTaller() || User.EsMecanico())
            lista.Add("  Finalizar Orden");

        // Solo JefeTaller y Recepcionista cancelan
        if (User.EsJefeTaller() || User.EsRecepcionista())
            lista.Add("  Cancelar Orden");

        lista.Add("  Volver al Menú Principal");
        return lista;
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
                Markup.Escape(o.MecanicoNombre ?? "-"),
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
        PrintHeader("Órdenes de Servicio", estado.HasValue ? $"Estado: {(EstadoOrden)estado}" : "Todas");

        PagedData<OrdenModel>? data = null;
        await WithSpinner("Cargando órdenes", async () => { data = await Api.GetOrdenesAsync(1, 20, estado); });

        if (data == null || data.Items.Count == 0) { NoData("No se encontraron órdenes."); Pause(); return; }

        MostrarTablaOrdenes(data.Items);
        Info($"Mostrando {data.Items.Count} de {data.TotalCount} órdenes");
        Pause();
    }

    // ── Filtrar por Estado ──────────────────────────────────────────────────

    private async Task FiltrarPorEstadoAsync()
    {
        PrintHeader("Filtrar por Estado");
        var estadoStr = Choice("  Selecciona el estado:",
            "Pendiente", "Aprobada", "EnProceso", "Finalizada", "Cancelada", "Todos");

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
        var numero = AskRequired("Número de orden:");
        OrdenModel? orden = null;

        await WithSpinner("Buscando", async () =>
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

        var grid = new Grid().AddColumn().AddColumn();
        void Row(string label, string value) =>
            grid.AddRow($"[grey]  {label}:[/]", $"[white]{value}[/]");

        Row("Numero",        orden.NumeroOrden);
        Row("Estado",        $"[{orden.EstadoColor}]{orden.EstadoTexto}[/]");
        Row("Cliente",       orden.ClienteNombre ?? "-");
        Row("Vehiculo",      orden.VehiculoPlaca ?? "-");
        Row("Mecanico",      orden.MecanicoNombre ?? "Sin asignar");
        Row("Descripcion",   orden.Descripcion ?? "-");
        Row("Fecha Ingreso", orden.FechaIngreso.ToString("dd/MM/yyyy HH:mm"));
        if (orden.FechaFin.HasValue) Row("Fecha Fin", orden.FechaFin.Value.ToString("dd/MM/yyyy HH:mm"));
        if (orden.Total.HasValue)    Row("Total",     $"$ {orden.Total:N2}");

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

        // Seleccionar cliente
        PagedData<ClienteModel>? clientes = null;
        await WithSpinner("Cargando clientes", async () => { clientes = await Api.GetClientesAsync(size: 50); });

        if (clientes == null || clientes.Items.Count == 0) { Error("No hay clientes registrados."); Pause(); return; }

        var opClientes = clientes.Items.Select(c => $"{c.NombreCompleto} — {c.NumeroDocumento}").ToList();
        opClientes.Add("Cancelar");
        var selCliente = Choice("Selecciona el cliente:", opClientes.ToArray());
        if (selCliente.Contains("Cancelar")) return;
        var cliente = clientes.Items[opClientes.IndexOf(selCliente)];

        // Seleccionar vehículo
        PagedData<VehiculoModel>? vehiculos = null;
        await WithSpinner("Cargando vehículos", async () => { vehiculos = await Api.GetVehiculosAsync(size: 50); });

        if (vehiculos == null || vehiculos.Items.Count == 0) { Error("No hay vehículos registrados."); Pause(); return; }

        var opVehiculos = vehiculos.Items.Select(v => $"{v.Placa} — {v.MarcaModelo} ({v.Anio})").ToList();
        opVehiculos.Add("Cancelar");
        var selVehiculo = Choice("Selecciona el vehículo:", opVehiculos.ToArray());
        if (selVehiculo.Contains("Cancelar")) return;
        var vehiculo = vehiculos.Items[opVehiculos.IndexOf(selVehiculo)];

        var desc = Ask("Descripción del trabajo (opcional):");
        if (!Confirm("Confirmar creacion de orden?")) return;

        OrdenModel? created = null;
        await WithSpinner("Creando orden", async () =>
        {
            created = await Api.CreateOrdenAsync(new
            {
                ClienteId = cliente.Id,
                VehiculoId = vehiculo.Id,
                Descripcion = string.IsNullOrEmpty(desc) ? null : desc
            });
        });

        if (created != null)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Panel(
                $"[grey]Numero:[/]  [bold cyan]{Markup.Escape(created.NumeroOrden)}[/]\n" +
                $"[grey]Cliente:[/] [white]{Markup.Escape(cliente.NombreCompleto)}[/]\n" +
                $"[grey]Placa:[/]   [white]{Markup.Escape(vehiculo.Placa)}[/]\n" +
                $"[grey]Estado:[/]  [yellow]Pendiente[/]")
                .Header("[bold green]  Orden Creada[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("green")));
        }
        else
            Error("No se pudo crear la orden.");
        Pause();
    }

    // ── Aprobar ─────────────────────────────────────────────────────────────

    private async Task AprobarAsync()
    {
        PrintHeader("Aprobar Orden — Iniciar Trabajo");

        var orden = await SeleccionarOrdenAsync(0); // Pendiente
        if (orden == null) return;

        if (!Confirm($"Aprobar [{orden.NumeroOrden}] e iniciar el proceso de trabajo?")) return;

        var (ok, error) = (false, "");
        await WithSpinner("Aprobando", async () =>
        {
            (ok, error) = await Api.AprobarOrdenAsync(orden.Id, orden.ClienteId);
        });

        if (ok) Ok("Orden aprobada. Ya puede asignarse mecánico y crear Mini-Órdenes.");
        else Error(error ?? "Error al aprobar.");
        Pause();
    }

    // ── Asignar Mecánico ────────────────────────────────────────────────────

    private async Task AsignarMecanicoAsync()
    {
        PrintHeader("Asignar Mecánico a Orden");

        var orden = await SeleccionarOrdenAsync();
        if (orden == null) return;

        PagedData<EmpleadoModel>? empleados = null;
        await WithSpinner("Cargando mecánicos", async () => { empleados = await Api.GetEmpleadosAsync(); });

        if (empleados == null || empleados.Items.Count == 0) { Error("No hay empleados registrados."); Pause(); return; }

        var opciones = empleados.Items.Select(e => $"{e.NombreCompleto}").ToList();
        opciones.Add("Cancelar");
        var sel = Choice("Selecciona el mecánico:", opciones.ToArray());
        if (sel.Contains("Cancelar")) return;

        var empleado = empleados.Items[opciones.IndexOf(sel)];
        if (!Confirm($"Asignar [{empleado.NombreCompleto}] a [{orden.NumeroOrden}]?")) return;

        var (ok, error) = (false, "");
        await WithSpinner("Asignando", async () => { (ok, error) = await Api.AsignarMecanicoAsync(orden.Id, empleado.Id); });

        if (ok) Ok($"Mecánico asignado correctamente a {orden.NumeroOrden}.");
        else Error(error ?? "Error al asignar.");
        Pause();
    }

    // ── Finalizar ───────────────────────────────────────────────────────────

    private async Task FinalizarAsync()
    {
        PrintHeader("Finalizar Orden");

        var orden = await SeleccionarOrdenAsync(2); // EnProceso
        if (orden == null) return;

        if (!Confirm($"Marcar [{orden.NumeroOrden}] como FINALIZADA?")) return;

        var (ok, error) = (false, "");
        await WithSpinner("Finalizando", async () => { (ok, error) = await Api.FinalizarOrdenAsync(orden.Id); });

        if (ok) Ok("Orden finalizada. Puede generarse la factura.");
        else Error(error ?? "Error al finalizar.");
        Pause();
    }

    // ── Cancelar ────────────────────────────────────────────────────────────

    private async Task CancelarAsync()
    {
        PrintHeader("Cancelar Orden");

        var orden = await SeleccionarOrdenAsync();
        if (orden == null) return;

        Warn("Esta accion no puede deshacerse.");
        var motivo = AskRequired("Motivo de cancelacion:");
        if (!Confirm($"Cancelar la orden [{orden.NumeroOrden}]?")) return;

        var (ok, error) = (false, "");
        await WithSpinner("Cancelando", async () => { (ok, error) = await Api.CancelarOrdenAsync(orden.Id, motivo); });

        if (ok) Ok("Orden cancelada.");
        else Error(error ?? "Error al cancelar.");
        Pause();
    }

    // ── Helper ──────────────────────────────────────────────────────────────

    private async Task<OrdenModel?> SeleccionarOrdenAsync(int? estadoFiltro = null)
    {
        PagedData<OrdenModel>? data = null;
        await WithSpinner("Cargando órdenes", async () => { data = await Api.GetOrdenesAsync(1, 50, estadoFiltro); });

        if (data == null || data.Items.Count == 0) { NoData("No se encontraron órdenes."); Pause(); return null; }

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
