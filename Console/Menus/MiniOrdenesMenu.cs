using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public class MiniOrdenesMenu : BaseMenu
{
    public MiniOrdenesMenu(ApiService api, AuthResponse user) : base(api, user) { }

    public async Task ShowAsync()
    {
        while (true)
        {
            PrintHeader("Mini-Órdenes", "Flujo Mecánico → Jefe → Cliente");

            var opciones = BuildOpciones();
            var opcion = Choice("  Selecciona una opción:", opciones.ToArray());

            if (opcion.Contains("Volver")) return;

            if (opcion.Contains("Listar")) await ListarAsync();
            else if (opcion.Contains("Crear")) await CrearAsync();
            else if (opcion.Contains("Enviar a revisión")) await EnviarRevisionAsync();
            else if (opcion.Contains("Aprobar / Rechazar (Jefe)")) await AprobarJefeAsync();
            else if (opcion.Contains("Aprobar / Rechazar (Cliente)")) await AprobarClienteAsync();
            else if (opcion.Contains("Completar")) await CompletarAsync();
        }
    }

    private List<string> BuildOpciones()
    {
        var lista = new List<string>
        {
            "  📋 Listar Mini-Órdenes"
        };

        if (User.EsMecanico())
        {
            lista.Add("  ➕ Crear Mini-Orden");
            lista.Add("  📤 Enviar a revisión del Jefe");
            lista.Add("  ✅ Completar Mini-Orden");
        }

        if (User.EsJefeTaller())
            lista.Add("  🔍 Aprobar / Rechazar (Jefe)");

        if (User.EsCliente() || User.EsAdmin() || User.EsRecepcionista())
            lista.Add("  ✔  Aprobar / Rechazar (Cliente)");

        lista.Add("  ← Volver al Menú Principal");
        return lista;
    }

    // ── Listar ───────────────────────────────────────────────────────────────

    private async Task ListarAsync()
    {
        PrintHeader("Mini-Órdenes", "Listado general");

        // Filtro por estado
        var estados = new[]
        {
            "Todos", "Borrador", "En Revisión Jefe", "Aprobada por Jefe",
            "En Revisión Cliente", "Aprobada por Cliente", "En Proceso",
            "Completada", "Rechazada"
        };

        var filtroStr = Choice("  Filtrar por estado:", estados);
        int? estadoNum = filtroStr switch
        {
            "Borrador"              => 0,
            "En Revisión Jefe"      => 1,
            "Aprobada por Jefe"     => 2,
            "En Revisión Cliente"   => 3,
            "Aprobada por Cliente"  => 4,
            "En Proceso"            => 5,
            "Completada"            => 6,
            "Rechazada"             => 7,
            _                       => null
        };

        PagedData<MiniOrdenModel>? data = null;
        await WithSpinner("Cargando mini-órdenes", async () =>
        {
            data = await Api.GetMiniOrdenesAsync(estado: estadoNum);
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData("No hay mini-órdenes con ese filtro.");
            Pause();
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .Expand();

        tabla.AddColumn(new TableColumn("[bold]Número[/]"));
        tabla.AddColumn(new TableColumn("[bold]Orden[/]"));
        tabla.AddColumn(new TableColumn("[bold]Área[/]"));
        tabla.AddColumn(new TableColumn("[bold]Estado[/]").Centered());
        tabla.AddColumn(new TableColumn("[bold]Mecánico[/]"));
        tabla.AddColumn(new TableColumn("[bold]Total[/]").RightAligned());
        tabla.AddColumn(new TableColumn("[bold]Fecha[/]"));

        foreach (var m in data.Items)
        {
            tabla.AddRow(
                $"[white]{Markup.Escape(m.NumeroMiniOrden)}[/]",
                $"[grey]{Markup.Escape(m.NumeroOrden ?? "-")}[/]",
                $"[grey]{Markup.Escape(m.AreaNombre ?? "General")}[/]",
                $"[{m.EstadoColor}]{Markup.Escape(m.EstadoNombre ?? m.Estado.ToString())}[/]",
                $"[cyan]{Markup.Escape(m.MecanicoNombre ?? "-")}[/]",
                $"[green]$ {m.Total:N2}[/]",
                $"[grey]{m.CreadoEn:dd/MM/yy}[/]"
            );
        }

        AnsiConsole.Write(tabla);
        AnsiConsole.MarkupLine($"\n[grey]  Total: {data.TotalCount} mini-orden(es)[/]");
        Pause();
    }

    // ── Crear ────────────────────────────────────────────────────────────────

    private async Task CrearAsync()
    {
        PrintHeader("Mini-Órdenes", "Crear nueva mini-orden");

        var numeroOrden = AskRequired("Número de Orden de Servicio (ej: OS-20260528-0001):");

        // Obtener la orden por número — buscamos en la lista
        PagedData<OrdenModel>? ordenes = null;
        await WithSpinner("Buscando orden", async () =>
        {
            ordenes = await Api.GetOrdenesAsync(size: 50);
        });

        var orden = ordenes?.Items.FirstOrDefault(o =>
            o.NumeroOrden.Equals(numeroOrden, StringComparison.OrdinalIgnoreCase));

        if (orden == null)
        {
            Error("Orden no encontrada. Verifica el número.");
            Pause();
            return;
        }

        Ok($"Orden encontrada: {orden.NumeroOrden} — Vehículo: {orden.VehiculoPlaca}");
        AnsiConsole.WriteLine();

        var descripcion = AskRequired("Descripción del trabajo:");
        var observaciones = Ask("Observaciones (opcional):");

        // Repuestos
        var repuestos = new List<object>();
        if (Confirm("¿Agregar repuestos?"))
        {
            do
            {
                var repuestoNombre = AskRequired("Buscar repuesto (nombre o código):");
                PagedData<RepuestoModel>? reps = null;
                await WithSpinner("Buscando", async () =>
                {
                    reps = await Api.GetRepuestosAsync(busqueda: repuestoNombre);
                });

                if (reps?.Items.Count == 0)
                {
                    Warn("No se encontraron repuestos.");
                    continue;
                }

                var opciones = reps!.Items.Select(r => $"{r.Codigo} - {r.Nombre} (Stock: {r.StockActual})").ToList();
                opciones.Add("← Cancelar");
                var sel = Choice("Selecciona el repuesto:", opciones.ToArray());
                if (sel.Contains("Cancelar")) break;

                var repSel = reps.Items[opciones.IndexOf(sel)];
                var cantidad = AskInt("Cantidad:", 1);
                var precio = AskDecimal("Precio unitario:", repSel.PrecioVenta);

                repuestos.Add(new { RepuestoId = repSel.Id, Cantidad = cantidad, PrecioUnitario = precio });
                Ok($"Repuesto agregado: {repSel.Nombre} x{cantidad}");

            } while (Confirm("¿Agregar otro repuesto?"));
        }

        MiniOrdenModel? result = null;
        string? errorMsg = null;

        await WithSpinner("Creando mini-orden", async () =>
        {
            result = await Api.CreateMiniOrdenAsync(new
            {
                OrdenServicioId = orden.Id,
                Descripcion = descripcion,
                Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones,
                Detalles = repuestos,
                ManosObra = (object?)null
            });
            if (result == null) errorMsg = "Error al crear la mini-orden.";
        });

        if (result != null)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Panel(
                $"[white]Número:[/] [cyan]{result.NumeroMiniOrden}[/]\n" +
                $"[white]Estado:[/] [{result.EstadoColor}]{result.EstadoNombre}[/]\n" +
                $"[white]Total:[/] [green]$ {result.Total:N2}[/]")
                .Header("[bold green]  ✓ Mini-Orden Creada[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("green")));
        }
        else
        {
            Error(errorMsg ?? "Error al crear.");
        }

        Pause();
    }

    // ── Enviar a revisión del Jefe ────────────────────────────────────────────

    private async Task EnviarRevisionAsync()
    {
        PrintHeader("Mini-Órdenes", "Enviar a revisión del Jefe de Taller");

        PagedData<MiniOrdenModel>? data = null;
        await WithSpinner("Cargando borradores", async () =>
        {
            data = await Api.GetMiniOrdenesAsync(estado: 0, mecanicoId: User.UserId);
        });

        var miniOrden = await SeleccionarMiniOrden(data, "Selecciona la mini-orden a enviar:");
        if (miniOrden == null) return;

        if (!Confirm($"¿Enviar [{miniOrden.NumeroMiniOrden}] a revisión del Jefe de Taller?")) return;

        MiniOrdenModel? result = null;
        await WithSpinner("Enviando", async () =>
        {
            result = await Api.EnviarRevisionJefeAsync(miniOrden.Id);
        });

        if (result != null)
            Ok($"{result.NumeroMiniOrden} enviada. Nuevo estado: {result.EstadoNombre}");
        else
            Error("No se pudo enviar a revisión.");

        Pause();
    }

    // ── Aprobar / Rechazar como Jefe ──────────────────────────────────────────

    private async Task AprobarJefeAsync()
    {
        PrintHeader("Mini-Órdenes", "Revisión del Jefe de Taller");

        PagedData<MiniOrdenModel>? data = null;
        await WithSpinner("Cargando pendientes", async () =>
        {
            data = await Api.GetMiniOrdenesAsync(estado: 1); // EnRevisionJefe
        });

        var miniOrden = await SeleccionarMiniOrden(data, "Selecciona la mini-orden a revisar:");
        if (miniOrden == null) return;

        MostrarResumenMiniOrden(miniOrden);

        var decision = Choice("¿Qué deseas hacer?",
            "  ✅ Aprobar y enviar al Cliente",
            "  ❌ Rechazar",
            "  ← Cancelar");

        if (decision.Contains("Cancelar")) return;

        bool aprobado = decision.Contains("Aprobar");
        string? obs = null;

        if (!aprobado)
            obs = AskRequired("Motivo del rechazo:");
        else
            obs = Ask("Observaciones (opcional):");

        MiniOrdenModel? result = null;
        await WithSpinner("Procesando", async () =>
        {
            result = await Api.AprobarRechazarJefeAsync(miniOrden.Id, aprobado,
                string.IsNullOrWhiteSpace(obs) ? null : obs);
        });

        if (result != null)
        {
            var color = aprobado ? "green" : "red";
            AnsiConsole.MarkupLine($"\n[{color}]  {(aprobado ? "✓ Aprobada" : "✗ Rechazada")} — Nuevo estado: {Markup.Escape(result.EstadoNombre ?? "")}[/]");
        }
        else
            Error("No se pudo procesar.");

        Pause();
    }

    // ── Aprobar / Rechazar como Cliente ───────────────────────────────────────

    private async Task AprobarClienteAsync()
    {
        PrintHeader("Mini-Órdenes", "Aprobación del Cliente");

        PagedData<MiniOrdenModel>? data = null;
        await WithSpinner("Cargando pendientes de aprobación", async () =>
        {
            data = await Api.GetMiniOrdenesAsync(estado: 3); // EnRevisionCliente
        });

        var miniOrden = await SeleccionarMiniOrden(data, "Selecciona la mini-orden a revisar:");
        if (miniOrden == null) return;

        MostrarResumenMiniOrden(miniOrden);

        var decision = Choice("¿Qué decide el cliente?",
            "  ✅ Aprobar — Se inicia el trabajo",
            "  ❌ Rechazar — No se ejecuta",
            "  ← Cancelar");

        if (decision.Contains("Cancelar")) return;

        bool aprobado = decision.Contains("Aprobar");
        string? obs = null;

        if (!aprobado)
            obs = AskRequired("Motivo del rechazo:");

        MiniOrdenModel? result = null;
        await WithSpinner("Procesando aprobación del cliente", async () =>
        {
            result = await Api.AprobarRechazarClienteAsync(miniOrden.Id, aprobado,
                string.IsNullOrWhiteSpace(obs) ? null : obs);
        });

        if (result != null)
        {
            var color = aprobado ? "green" : "red";
            AnsiConsole.MarkupLine($"\n[{color}]  {(aprobado ? "✓ Cliente aprobó — trabajo en proceso" : "✗ Cliente rechazó")}[/]");
            AnsiConsole.MarkupLine($"[grey]  Nuevo estado: {Markup.Escape(result.EstadoNombre ?? "")}[/]");
        }
        else
            Error("No se pudo procesar.");

        Pause();
    }

    // ── Completar ─────────────────────────────────────────────────────────────

    private async Task CompletarAsync()
    {
        PrintHeader("Mini-Órdenes", "Completar trabajo");

        PagedData<MiniOrdenModel>? data = null;
        await WithSpinner("Cargando en proceso", async () =>
        {
            data = await Api.GetMiniOrdenesAsync(estado: 5, mecanicoId: User.UserId); // EnProceso
        });

        var miniOrden = await SeleccionarMiniOrden(data, "Selecciona la mini-orden completada:");
        if (miniOrden == null) return;

        MostrarResumenMiniOrden(miniOrden);

        if (!Confirm("¿Marcar esta mini-orden como COMPLETADA?")) return;

        var obs = Ask("Observaciones finales (opcional):");

        MiniOrdenModel? result = null;
        await WithSpinner("Completando", async () =>
        {
            result = await Api.CompletarMiniOrdenAsync(miniOrden.Id,
                string.IsNullOrWhiteSpace(obs) ? null : obs);
        });

        if (result != null)
            Ok($"Mini-orden {result.NumeroMiniOrden} completada exitosamente.");
        else
            Error("No se pudo completar.");

        Pause();
    }

    // ── Helpers visuales ─────────────────────────────────────────────────────

    private async Task<MiniOrdenModel?> SeleccionarMiniOrden(PagedData<MiniOrdenModel>? data, string titulo)
    {
        if (data == null || data.Items.Count == 0)
        {
            NoData("No hay mini-órdenes disponibles con ese estado.");
            Pause();
            return null;
        }

        var opciones = data.Items
            .Select(m => $"{m.NumeroMiniOrden}  [{m.EstadoNombre}]  — {m.Descripcion[..Math.Min(40, m.Descripcion.Length)]}")
            .ToList();
        opciones.Add("← Cancelar");

        var sel = Choice(titulo, opciones.ToArray());
        if (sel.Contains("Cancelar")) return null;

        return data.Items[opciones.IndexOf(sel)];
    }

    private static void MostrarResumenMiniOrden(MiniOrdenModel m)
    {
        var panel = new Panel(
            $"[grey]Orden:[/]      [white]{Markup.Escape(m.NumeroOrden ?? "-")}[/]\n" +
            $"[grey]Área:[/]       [white]{Markup.Escape(m.AreaNombre ?? "General")}[/]\n" +
            $"[grey]Estado:[/]     [{m.EstadoColor}]{Markup.Escape(m.EstadoNombre ?? "")}[/]\n" +
            $"[grey]Mecánico:[/]   [cyan]{Markup.Escape(m.MecanicoNombre ?? "-")}[/]\n" +
            $"[grey]Descripción:[/] {Markup.Escape(m.Descripcion)}\n" +
            $"[grey]Materiales:[/] [green]$ {m.TotalMateriales:N2}[/]\n" +
            $"[grey]Mano Obra:[/]  [green]$ {m.TotalManoObra:N2}[/]\n" +
            $"[grey]Total:[/]      [bold green]$ {m.Total:N2}[/]")
            .Header($"[bold cyan]  {Markup.Escape(m.NumeroMiniOrden)}[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("blue"));

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }
}
