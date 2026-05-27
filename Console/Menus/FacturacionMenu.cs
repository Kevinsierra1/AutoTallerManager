using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public class FacturacionMenu : BaseMenu
{
    public FacturacionMenu(ApiService api, AuthResponse user) : base(api, user) { }

    public async Task ShowAsync()
    {
        while (true)
        {
            PrintHeader("Facturación");

            var opcion = Choice("  Facturación:",
                "  Listar Facturas",
                "  Ver Detalle de Factura",
                "  Generar Factura (desde Orden Finalizada)",
                "  Volver al Menú Principal");

            if (opcion.Contains("Volver")) return;

            if (opcion.Contains("Listar"))    await ListarAsync();
            else if (opcion.Contains("Detalle"))   await VerDetalleAsync();
            else if (opcion.Contains("Generar"))   await GenerarAsync();
        }
    }

    // ── Listar ──────────────────────────────────────────────────────────────

    private async Task ListarAsync()
    {
        PrintHeader("Facturas", "Listado de facturas emitidas");

        PagedData<FacturaModel>? data = null;
        await WithSpinner("Cargando facturas", async () =>
        {
            data = await Api.GetFacturasAsync(1, 20);
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData("No hay facturas emitidas.");
            Pause();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .Expand();

        table.AddColumn(new TableColumn("[bold]# Factura[/]"));
        table.AddColumn(new TableColumn("[bold]Cliente[/]"));
        table.AddColumn(new TableColumn("[bold]Subtotal[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Impuestos[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Descuento[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Total[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Estado[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Emisión[/]").Centered());

        foreach (var f in data.Items)
        {
            var estadoPago = f.Pagada ? "[green]Pagada[/]" : "[yellow]Pendiente[/]";
            table.AddRow(
                $"[bold]{Markup.Escape(f.NumeroFactura)}[/]",
                Markup.Escape(f.ClienteNombre ?? "-"),
                $"$ {f.Subtotal:N2}",
                $"$ {f.Impuestos:N2}",
                f.Descuento > 0 ? $"[green]-$ {f.Descuento:N2}[/]" : "-",
                $"[bold]$ {f.Total:N2}[/]",
                estadoPago,
                f.FechaEmision.ToString("dd/MM/yyyy")
            );
        }

        AnsiConsole.Write(table);
        Info($"Mostrando {data.Items.Count} de {data.TotalCount} facturas");
        Pause();
    }

    // ── Ver Detalle ─────────────────────────────────────────────────────────

    private async Task VerDetalleAsync()
    {
        PrintHeader("Detalle de Factura");

        var numStr = AskRequired("Número de factura o ID (GUID):");
        FacturaModel? factura = null;

        await WithSpinner("Buscando factura", async () =>
        {
            if (Guid.TryParse(numStr, out var id))
            {
                factura = await Api.GetFacturaByIdAsync(id);
            }
            else
            {
                // Buscar por número en la lista
                var lista = await Api.GetFacturasAsync(1, 100);
                factura = lista?.Items.FirstOrDefault(f =>
                    f.NumeroFactura.Contains(numStr, StringComparison.OrdinalIgnoreCase));
            }
        });

        if (factura == null)
        {
            NoData("Factura no encontrada.");
            Pause();
            return;
        }

        var grid = new Grid();
        grid.AddColumn(); grid.AddColumn();

        void Row(string label, string value) =>
            grid.AddRow($"[grey]  {label}:[/]", $"[white]{value}[/]");

        Row("Número", factura.NumeroFactura);
        Row("Cliente", factura.ClienteNombre ?? "-");
        Row("Orden ID", factura.OrdenServicioId.ToString());
        Row("Fecha Emisión", factura.FechaEmision.ToString("dd/MM/yyyy HH:mm"));
        Row("Subtotal", $"$ {factura.Subtotal:N2}");
        Row("Impuestos (IVA)", $"$ {factura.Impuestos:N2}");
        if (factura.Descuento > 0) Row("Descuento", $"-$ {factura.Descuento:N2}");
        Row("TOTAL", $"[bold green]$ {factura.Total:N2}[/]");
        Row("Estado de Pago", factura.Pagada ? "[green]PAGADA[/]" : "[yellow]PENDIENTE DE PAGO[/]");

        AnsiConsole.Write(new Panel(grid)
            .Header($"[bold]  Factura #{Markup.Escape(factura.NumeroFactura)}[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("blue")));

        Pause();
    }

    // ── Generar ─────────────────────────────────────────────────────────────

    private async Task GenerarAsync()
    {
        PrintHeader("Generar Factura");

        Info("Solo se puede facturar órdenes en estado [green]Finalizada[/].");
        AnsiConsole.WriteLine();

        // Mostrar órdenes finalizadas
        PagedData<OrdenModel>? ordenes = null;
        await WithSpinner("Cargando órdenes finalizadas", async () =>
        {
            ordenes = await Api.GetOrdenesAsync(1, 50, 3); // Finalizada = 3
        });

        if (ordenes == null || ordenes.Items.Count == 0)
        {
            NoData("No hay órdenes finalizadas pendientes de facturar.");
            Pause();
            return;
        }

        var opciones = ordenes.Items
            .Select(o => $"{o.NumeroOrden}  |  {o.ClienteNombre ?? "-"}  |  {o.VehiculoPlaca ?? "-"}  |  Total: {(o.Total.HasValue ? $"${o.Total:N2}" : "N/D")}")
            .ToList();
        opciones.Add("Cancelar");

        var sel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Selecciona la orden a facturar:[/]")
                .PageSize(12)
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices(opciones));

        if (sel == "Cancelar") return;

        var orden = ordenes.Items[opciones.IndexOf(sel)];

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[grey]  Orden:[/] [bold]{Markup.Escape(orden.NumeroOrden)}[/]");
        AnsiConsole.MarkupLine($"[grey]  Cliente:[/] {Markup.Escape(orden.ClienteNombre ?? "-")}");
        AnsiConsole.MarkupLine($"[grey]  Total Orden:[/] {(orden.Total.HasValue ? $"[bold]$ {orden.Total:N2}[/]" : "[grey]No calculado[/]")}");
        AnsiConsole.WriteLine();

        var descuento = AskDecimal("Descuento a aplicar ($ o 0 para ninguno):", 0);

        if (!Confirm("¿Generar la factura?")) return;

        FacturaModel? factura = null;
        await WithSpinner("Generando factura", async () =>
        {
            factura = await Api.GenerarFacturaAsync(orden.Id, descuento);
        });

        if (factura != null)
        {
            AnsiConsole.WriteLine();
            Ok($"Factura [bold]{factura.NumeroFactura}[/] generada correctamente.");
            AnsiConsole.MarkupLine($"[grey]  Total:[/] [bold green]$ {factura.Total:N2}[/]");
        }
        else
        {
            Error("No se pudo generar la factura. Verifica que la orden no tenga factura ya emitida.");
        }

        Pause();
    }
}
