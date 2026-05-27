using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public class InventarioMenu : BaseMenu
{
    public InventarioMenu(ApiService api, AuthResponse user) : base(api, user) { }

    public async Task ShowAsync()
    {
        while (true)
        {
            PrintHeader("Inventario y Repuestos");

            var opcion = Choice("  Inventario:",
                "  Listar Repuestos",
                "  Buscar Repuesto",
                "  Stock Crítico",
                "  Crear Repuesto",
                "  Actualizar Repuesto",
                "  Eliminar Repuesto",
                "  Registrar Entrada de Stock",
                "  Registrar Salida de Stock",
                "  Ver Movimientos",
                "  Volver al Menú Principal");

            if (opcion.Contains("Volver")) return;

            if (opcion.Contains("Listar"))           await ListarRepuestosAsync();
            else if (opcion.Contains("Buscar"))           await BuscarRepuestoAsync();
            else if (opcion.Contains("Crítico"))          await StockCriticoAsync();
            else if (opcion.Contains("Crear"))            await CrearRepuestoAsync();
            else if (opcion.Contains("Actualizar"))       await ActualizarRepuestoAsync();
            else if (opcion.Contains("Eliminar"))         await EliminarRepuestoAsync();
            else if (opcion.Contains("Entrada"))          await EntradaStockAsync();
            else if (opcion.Contains("Salida"))           await SalidaStockAsync();
            else if (opcion.Contains("Movimientos"))      await VerMovimientosAsync();
        }
    }

    // ── Tabla de repuestos ──────────────────────────────────────────────────

    private static void MostrarTablaRepuestos(IEnumerable<RepuestoModel> items)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .Expand();

        table.AddColumn(new TableColumn("[bold]Código[/]"));
        table.AddColumn(new TableColumn("[bold]Nombre[/]"));
        table.AddColumn(new TableColumn("[bold]Categoría[/]"));
        table.AddColumn(new TableColumn("[bold]Stock[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Mín.[/]").Centered());
        table.AddColumn(new TableColumn("[bold]P. Venta[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Estado[/]").Centered());

        foreach (var r in items)
        {
            var stockColor = r.StockCritico ? "red" : "green";
            var estado = r.Activo ? "[green]Activo[/]" : "[red]Inactivo[/]";

            table.AddRow(
                $"[cyan]{Markup.Escape(r.Codigo)}[/]",
                Markup.Escape(r.Nombre),
                Markup.Escape(r.Categoria ?? "-"),
                $"[{stockColor}]{r.StockActual}[/]",
                r.StockMinimo.ToString(),
                $"$ {r.PrecioVenta:N2}",
                estado
            );
        }

        AnsiConsole.Write(table);
    }

    // ── Listar ──────────────────────────────────────────────────────────────

    private async Task ListarRepuestosAsync(string? busqueda = null, bool? critico = null)
    {
        var titulo = critico == true ? "Stock Crítico" : (busqueda != null ? $"Búsqueda: \"{busqueda}\"" : "Todos los Repuestos");
        PrintHeader("Repuestos", titulo);

        PagedData<RepuestoModel>? data = null;
        await WithSpinner("Cargando repuestos", async () =>
        {
            data = await Api.GetRepuestosAsync(1, 30, busqueda, critico);
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData("No se encontraron repuestos.");
            Pause();
            return;
        }

        MostrarTablaRepuestos(data.Items);
        Info($"Mostrando {data.Items.Count} de {data.TotalCount} repuestos");
        Pause();
    }

    private async Task BuscarRepuestoAsync()
    {
        PrintHeader("Buscar Repuesto");
        var b = AskRequired("Ingresa código, nombre o descripción:");
        await ListarRepuestosAsync(b);
    }

    private async Task StockCriticoAsync() => await ListarRepuestosAsync(null, true);

    // ── Crear ───────────────────────────────────────────────────────────────

    private async Task CrearRepuestoAsync()
    {
        PrintHeader("Crear Repuesto");

        var codigo = AskRequired("Código (ej: REP-001):");
        var nombre = AskRequired("Nombre:");
        var desc   = Ask("Descripción (opcional):");

        Info("CategoriaRepuestoId: busca los IDs disponibles en la BD (seed cargó: Filtros, Frenos, Motor, Suspensión, Eléctrico, Lubricantes).");
        var catIdStr = AskRequired("CategoriaRepuestoId (GUID):");
        if (!Guid.TryParse(catIdStr, out var catId)) { Error("GUID inválido."); Pause(); return; }

        var precioCompra = AskDecimal("Precio de Compra ($):", 0);
        var precioVenta  = AskDecimal("Precio de Venta ($):", 0);
        var stockActual  = AskInt("Stock Inicial:", 0);
        var stockMinimo  = AskInt("Stock Mínimo:", 5);
        var unidad       = Ask("Unidad (unidad/litro/kit, opcional):");

        AnsiConsole.WriteLine();
        if (!Confirm("¿Crear este repuesto?")) return;

        RepuestoModel? created = null;
        await WithSpinner("Creando repuesto", async () =>
        {
            created = await Api.CreateRepuestoAsync(new
            {
                Codigo = codigo, Nombre = nombre,
                Descripcion = string.IsNullOrEmpty(desc) ? null : desc,
                CategoriaRepuestoId = catId,
                PrecioCompra = precioCompra, PrecioVenta = precioVenta,
                StockActual = stockActual, StockMinimo = stockMinimo,
                Unidad = string.IsNullOrEmpty(unidad) ? null : unidad
            });
        });

        if (created != null) Ok($"Repuesto '[bold]{Markup.Escape(created.Nombre)}[/]' creado. Stock: {created.StockActual}");
        else Error("No se pudo crear el repuesto.");
        Pause();
    }

    // ── Actualizar ──────────────────────────────────────────────────────────

    private async Task ActualizarRepuestoAsync()
    {
        PrintHeader("Actualizar Repuesto");

        var rep = await SeleccionarRepuestoAsync();
        if (rep == null) return;

        AnsiConsole.WriteLine();
        Info("Deja vacío para mantener el valor actual.");
        AnsiConsole.WriteLine();

        var nombre      = Ask($"Nombre [{rep.Nombre}]:", rep.Nombre);
        var desc        = Ask($"Descripción [{rep.Descripcion ?? "vacío"}]:", rep.Descripcion ?? "");
        var precioCompra = AskDecimal($"Precio Compra [{rep.PrecioCompra}]:", rep.PrecioCompra);
        var precioVenta  = AskDecimal($"Precio Venta [{rep.PrecioVenta}]:", rep.PrecioVenta);
        var stockMin    = AskInt($"Stock Mínimo [{rep.StockMinimo}]:", rep.StockMinimo);
        var activo      = Confirm($"¿Activo? (actualmente: {rep.Activo})");

        if (!Confirm("¿Guardar cambios?")) return;

        RepuestoModel? updated = null;
        await WithSpinner("Actualizando", async () =>
        {
            updated = await Api.UpdateRepuestoAsync(rep.Id, new
            {
                Nombre = nombre, Descripcion = string.IsNullOrEmpty(desc) ? null : desc,
                PrecioCompra = precioCompra, PrecioVenta = precioVenta,
                StockMinimo = stockMin, Activo = activo,
                Unidad = rep.Unidad
            });
        });

        if (updated != null) Ok("Repuesto actualizado.");
        else Error("No se pudo actualizar.");
        Pause();
    }

    // ── Eliminar ────────────────────────────────────────────────────────────

    private async Task EliminarRepuestoAsync()
    {
        PrintHeader("Eliminar Repuesto");

        var rep = await SeleccionarRepuestoAsync();
        if (rep == null) return;

        if (!Confirm($"¿Eliminar el repuesto '{rep.Nombre}'?")) return;

        (bool ok, string? error) result = (false, null);
        await WithSpinner("Eliminando", async () => { result = await Api.DeleteRepuestoAsync(rep.Id); });

        if (result.ok) Ok("Repuesto eliminado.");
        else Error(result.error ?? "Error al eliminar.");
        Pause();
    }

    // ── Entrada ─────────────────────────────────────────────────────────────

    private async Task EntradaStockAsync()
    {
        PrintHeader("Entrada de Inventario");

        var rep = await SeleccionarRepuestoAsync();
        if (rep == null) return;

        AnsiConsole.MarkupLine($"[grey]  Stock actual:[/] [cyan]{rep.StockActual}[/]");
        var cantidad = AskInt("Cantidad a ingresar:", 1);
        var motivo   = Ask("Motivo (compra, ajuste...):");

        if (!Confirm($"¿Registrar entrada de {cantidad} unidad(es) de '{rep.Nombre}'?")) return;

        (bool ok, string? error) result = (false, null);
        await WithSpinner("Registrando entrada", async () =>
        {
            result = await Api.EntradaInventarioAsync(rep.Id, cantidad, string.IsNullOrEmpty(motivo) ? null : motivo);
        });

        if (result.ok) Ok($"Entrada registrada. Nuevo stock: {rep.StockActual + cantidad}");
        else Error(result.error ?? "Error al registrar.");
        Pause();
    }

    // ── Salida ──────────────────────────────────────────────────────────────

    private async Task SalidaStockAsync()
    {
        PrintHeader("Salida de Inventario");

        var rep = await SeleccionarRepuestoAsync();
        if (rep == null) return;

        AnsiConsole.MarkupLine($"[grey]  Stock actual:[/] [cyan]{rep.StockActual}[/]");
        var cantidad = AskInt("Cantidad a retirar:", 1);
        var motivo   = Ask("Motivo (uso en orden, etc.):");

        if (cantidad > rep.StockActual)
        {
            Warn($"Stock insuficiente. Disponible: {rep.StockActual}");
            Pause();
            return;
        }

        if (!Confirm($"¿Registrar salida de {cantidad} unidad(es) de '{rep.Nombre}'?")) return;

        (bool ok, string? error) result = (false, null);
        await WithSpinner("Registrando salida", async () =>
        {
            result = await Api.SalidaInventarioAsync(rep.Id, cantidad, string.IsNullOrEmpty(motivo) ? null : motivo);
        });

        if (result.ok) Ok($"Salida registrada. Nuevo stock: {rep.StockActual - cantidad}");
        else Error(result.error ?? "Error al registrar.");
        Pause();
    }

    // ── Ver Movimientos ─────────────────────────────────────────────────────

    private async Task VerMovimientosAsync()
    {
        PrintHeader("Movimientos de Inventario");

        PagedData<MovimientoModel>? data = null;
        await WithSpinner("Cargando movimientos", async () =>
        {
            data = await Api.GetMovimientosAsync(1, 30);
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData("No hay movimientos registrados.");
            Pause();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .Expand();

        table.AddColumn(new TableColumn("[bold]Repuesto[/]"));
        table.AddColumn(new TableColumn("[bold]Tipo[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Cantidad[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Anterior[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Nuevo[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Motivo[/]"));
        table.AddColumn(new TableColumn("[bold]Fecha[/]").Centered());

        foreach (var m in data.Items)
        {
            table.AddRow(
                Markup.Escape(m.RepuestoNombre ?? "-"),
                $"[{m.TipoColor}]{Markup.Escape(m.TipoTexto)}[/]",
                m.Cantidad.ToString(),
                m.CantidadAnterior.ToString(),
                m.CantidadNueva.ToString(),
                Markup.Escape(m.Motivo ?? "-"),
                m.FechaMovimiento.ToString("dd/MM/yy HH:mm")
            );
        }

        AnsiConsole.Write(table);
        Info($"Mostrando {data.Items.Count} de {data.TotalCount} movimientos");
        Pause();
    }

    // ── Helper: seleccionar repuesto ────────────────────────────────────────

    private async Task<RepuestoModel?> SeleccionarRepuestoAsync(string? busqueda = null)
    {
        if (busqueda == null)
        {
            busqueda = AskRequired("Busca el repuesto (código/nombre):");
        }

        PagedData<RepuestoModel>? data = null;
        await WithSpinner("Buscando", async () =>
        {
            data = await Api.GetRepuestosAsync(1, 20, busqueda);
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData("No se encontraron repuestos.");
            Pause();
            return null;
        }

        var opciones = data.Items
            .Select(r => $"[cyan]{r.Codigo}[/]  {Markup.Escape(r.Nombre)}  (stock: {r.StockActual})")
            .ToList();
        opciones.Add("Cancelar");

        var sel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Selecciona el repuesto:[/]")
                .PageSize(10)
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices(opciones));

        if (sel == "Cancelar") return null;
        return data.Items[opciones.IndexOf(sel)];
    }
}
