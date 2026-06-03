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
            PrintHeader("Presupuestos", "Diagnóstico → Aprobación Jefe → Aprobación Cliente → Orden de Servicio");

            var opciones = BuildOpciones();
            var opcion = Choice("  Selecciona una opción:", opciones.ToArray());

            if (opcion.Contains("Volver")) return;

            if (opcion.Contains("Listar"))                         await ListarAsync();
            else if (opcion.Contains("Crear Presupuesto"))         await CrearAsync();
            else if (opcion.Contains("Enviar a Jefe"))             await EnviarRevisionAsync();
            else if (opcion.Contains("Revisar como Jefe"))         await AprobarJefeAsync();
            else if (opcion.Contains("Aprobar como Cliente"))      await AprobarClienteAsync();
            else if (opcion.Contains("Completar trabajo"))         await CompletarAsync();
        }
    }

    private List<string> BuildOpciones()
    {
        var lista = new List<string> { "  Ver todos los presupuestos" };

        if (User.EsMecanico())
        {
            lista.Add("  Crear Presupuesto (nuevo diagnóstico)");
            lista.Add("  Enviar a Jefe de Taller para revisión");
            lista.Add("  Completar trabajo");
        }

        if (User.EsJefeTaller())
            lista.Add("  Revisar como Jefe — Aprobar o Rechazar");

        if (User.EsCliente() || User.EsAdmin() || User.EsRecepcionista())
            lista.Add("  Aprobar como Cliente — genera la Orden de Servicio");

        lista.Add("  ← Volver al Menú Principal");
        return lista;
    }

    // ── Listar ───────────────────────────────────────────────────────────────

    private async Task ListarAsync()
    {
        PrintHeader("Presupuestos", "Listado");

        var estados = new[]
        {
            "Todos", "Borrador", "En Revisión Jefe", "Aprobado por Jefe",
            "Enviado al Cliente", "Aprobado por Cliente (OS generada)", "En Proceso", "Completado", "Rechazado"
        };
        var filtroStr = Choice("  Filtrar por estado:", estados);
        int? estadoNum = filtroStr switch
        {
            "Borrador"                          => 0,
            "En Revisión Jefe"                  => 1,
            "Aprobado por Jefe"                 => 2,
            "Enviado al Cliente"                => 3,
            "Aprobado por Cliente (OS generada)"=> 4,
            "En Proceso"                        => 5,
            "Completado"                        => 6,
            "Rechazado"                         => 7,
            _                                   => null
        };

        PagedData<MiniOrdenModel>? data = null;
        await WithSpinner("Cargando presupuestos", async () =>
        {
            data = await Api.GetMiniOrdenesAsync(estado: estadoNum);
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData("No hay presupuestos con ese filtro.");
            Pause();
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .Expand();

        tabla.AddColumn("[bold]Número[/]");
        tabla.AddColumn("[bold]Cliente[/]");
        tabla.AddColumn("[bold]Vehículo[/]");
        tabla.AddColumn(new TableColumn("[bold]Estado[/]").Centered());
        tabla.AddColumn("[bold]Mecánico[/]");
        tabla.AddColumn(new TableColumn("[bold]Total[/]").RightAligned());
        tabla.AddColumn("[bold]OS Generada[/]");

        foreach (var m in data.Items)
        {
            tabla.AddRow(
                $"[white]{Markup.Escape(m.NumeroMiniOrden)}[/]",
                $"[cyan]{Markup.Escape(m.MecanicoNombre ?? "-")}[/]",  // ClienteNombre en nuevo modelo
                $"[grey]{Markup.Escape(m.AreaNombre ?? "-")}[/]",       // VehiculoPlaca
                $"[{m.EstadoColor}]{Markup.Escape(m.EstadoNombre ?? m.Estado.ToString())}[/]",
                $"[grey]{Markup.Escape(m.MecanicoNombre ?? "-")}[/]",
                $"[green]$ {m.Total:N2}[/]",
                m.Estado >= 4 ? "[green]Si[/]" : "[grey]No[/]"
            );
        }

        AnsiConsole.Write(tabla);
        AnsiConsole.MarkupLine($"\n[grey]  Total: {data.TotalCount} presupuesto(s)[/]");
        Pause();
    }

    // ── Crear Presupuesto ────────────────────────────────────────────────────

    private async Task CrearAsync()
    {
        PrintHeader("Presupuestos", "Crear nuevo presupuesto de diagnóstico");

        // 1. Seleccionar cliente
        PagedData<ClienteModel>? clientes = null;
        await WithSpinner("Cargando clientes", async () =>
        {
            clientes = await Api.GetClientesAsync(size: 50);
        });

        if (clientes == null || clientes.Items.Count == 0)
        {
            Error("No hay clientes registrados.");
            Pause();
            return;
        }

        var opClientes = clientes.Items.Select(c => $"{c.NombreCompleto} — {c.NumeroDocumento}").ToList();
        opClientes.Add("Cancelar");
        var selCliente = Choice("Selecciona el cliente:", opClientes.ToArray());
        if (selCliente.Contains("Cancelar")) return;
        var cliente = clientes.Items[opClientes.IndexOf(selCliente)];

        // 2. Seleccionar vehículo
        PagedData<VehiculoModel>? vehiculos = null;
        await WithSpinner("Cargando vehículos", async () =>
        {
            vehiculos = await Api.GetVehiculosAsync(size: 50);
        });

        if (vehiculos == null || vehiculos.Items.Count == 0)
        {
            Error("No hay vehículos registrados.");
            Pause();
            return;
        }

        var opVehiculos = vehiculos.Items.Select(v => $"{v.Placa} — {v.MarcaModelo} ({v.Anio})").ToList();
        opVehiculos.Add("Cancelar");
        var selVehiculo = Choice("Selecciona el vehículo:", opVehiculos.ToArray());
        if (selVehiculo.Contains("Cancelar")) return;
        var vehiculo = vehiculos.Items[opVehiculos.IndexOf(selVehiculo)];

        // 3. Descripción del diagnóstico
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]  Describe qué reparaciones o servicios necesita el vehículo:[/]");
        var descripcion = AskRequired("Descripción del trabajo:");
        var observaciones = Ask("Observaciones adicionales (opcional):");

        // 4. Agregar repuestos (opcional)
        var repuestos = new List<object>();
        if (Confirm("Agregar repuestos o materiales al presupuesto?"))
        {
            do
            {
                var busqueda = AskRequired("Buscar repuesto (nombre o código):");
                PagedData<RepuestoModel>? reps = null;
                await WithSpinner("Buscando", async () =>
                {
                    reps = await Api.GetRepuestosAsync(busqueda: busqueda);
                });

                if (reps?.Items.Count == 0) { Warn("No se encontraron repuestos con ese criterio."); continue; }

                var opRep = reps!.Items.Select(r => $"{r.Codigo} — {r.Nombre}  (Stock: {r.StockActual}, Precio: ${r.PrecioVenta:N0})").ToList();
                opRep.Add("Cancelar");
                var selRep = Choice("Selecciona el repuesto:", opRep.ToArray());
                if (selRep.Contains("Cancelar")) break;

                var rep = reps.Items[opRep.IndexOf(selRep)];
                var cantidad = AskInt("Cantidad:", 1);
                var precio = AskDecimal("Precio unitario:", rep.PrecioVenta);
                repuestos.Add(new { RepuestoId = rep.Id, Cantidad = cantidad, PrecioUnitario = precio });
                Ok($"Agregado: {rep.Nombre} x{cantidad} = ${cantidad * precio:N2}");

            } while (Confirm("Agregar otro repuesto?"));
        }

        if (!Confirm($"Crear presupuesto para [{cliente.NombreCompleto}] — [{vehiculo.Placa}]?")) return;

        MiniOrdenModel? result = null;
        await WithSpinner("Creando presupuesto", async () =>
        {
            result = await Api.CreatePresupuestoAsync(
                cliente.Id, vehiculo.Id, descripcion,
                string.IsNullOrWhiteSpace(observaciones) ? null : observaciones,
                repuestos.Count > 0 ? repuestos : null);
        });

        if (result != null)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Panel(
                $"[grey]Numero:[/]    [bold cyan]{Markup.Escape(result.NumeroMiniOrden)}[/]\n" +
                $"[grey]Estado:[/]    [{result.EstadoColor}]{Markup.Escape(result.EstadoNombre ?? "Borrador")}[/]\n" +
                $"[grey]Total:[/]     [green]$ {result.Total:N2}[/]\n\n" +
                $"[grey]Siguiente paso:[/] Envíalo al Jefe de Taller para revisión.")
                .Header("[bold green]  Presupuesto Creado[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("green")));
        }
        else
            Error("No se pudo crear el presupuesto.");

        Pause();
    }

    // ── Enviar a revisión del Jefe ────────────────────────────────────────────

    private async Task EnviarRevisionAsync()
    {
        PrintHeader("Presupuestos", "Enviar al Jefe de Taller para revisión");

        PagedData<MiniOrdenModel>? data = null;
        await WithSpinner("Cargando borradores", async () =>
        {
            data = await Api.GetMiniOrdenesAsync(estado: 0);
        });

        var pres = await SeleccionarPresupuesto(data, "Selecciona el presupuesto a enviar:");
        if (pres == null) return;

        if (!Confirm($"Enviar [{pres.NumeroMiniOrden}] al Jefe de Taller?")) return;

        MiniOrdenModel? result = null;
        await WithSpinner("Enviando", async () =>
        {
            result = await Api.EnviarRevisionJefeAsync(pres.Id);
        });

        if (result != null)
            Ok($"Enviado. Estado: {result.EstadoNombre}");
        else
            Error("No se pudo enviar.");

        Pause();
    }

    // ── Aprobar / Rechazar como Jefe ──────────────────────────────────────────

    private async Task AprobarJefeAsync()
    {
        PrintHeader("Presupuestos", "Revisión del Jefe de Taller");

        PagedData<MiniOrdenModel>? data = null;
        await WithSpinner("Cargando pendientes", async () =>
        {
            data = await Api.GetMiniOrdenesAsync(estado: 1);
        });

        var pres = await SeleccionarPresupuesto(data, "Selecciona el presupuesto a revisar:");
        if (pres == null) return;

        MostrarResumen(pres);

        var decision = Choice("Decision del Jefe:",
            "  Aprobar — enviar al cliente para su aprobacion final",
            "  Rechazar — devolver al mecanico",
            "  Cancelar");
        if (decision.Contains("Cancelar")) return;

        bool aprobado = decision.Contains("Aprobar");
        string? obs = aprobado
            ? Ask("Observaciones para el cliente (opcional):")
            : AskRequired("Motivo del rechazo:");

        MiniOrdenModel? result = null;
        await WithSpinner("Procesando", async () =>
        {
            result = await Api.AprobarRechazarJefeAsync(pres.Id, aprobado,
                string.IsNullOrWhiteSpace(obs) ? null : obs);
        });

        if (result != null)
        {
            var color = aprobado ? "green" : "red";
            AnsiConsole.MarkupLine($"\n[{color}]  {(aprobado ? "Aprobado — listo para enviar al cliente" : "Rechazado — el mecanico debe corregirlo")}[/]");
        }
        else Error("No se pudo procesar.");

        Pause();
    }

    // ── Aprobar / Rechazar como Cliente ── GENERA LA OS ───────────────────────

    private async Task AprobarClienteAsync()
    {
        PrintHeader("Presupuestos", "Aprobacion del Cliente — genera la Orden de Servicio");

        PagedData<MiniOrdenModel>? data = null;
        await WithSpinner("Cargando presupuestos pendientes de aprobacion", async () =>
        {
            data = await Api.GetMiniOrdenesAsync(estado: 3);
        });

        var pres = await SeleccionarPresupuesto(data, "Selecciona el presupuesto:");
        if (pres == null) return;

        MostrarResumen(pres);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Panel(
            "[white]Si el cliente aprueba este presupuesto:[/]\n\n" +
            "[green]  Se creara automaticamente la Orden de Servicio[/]\n" +
            "[green]  El mecanico podra comenzar el trabajo de inmediato[/]")
            .Header("[bold yellow]  Importante[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("yellow")));
        AnsiConsole.WriteLine();

        var decision = Choice("Decision del cliente:",
            "  Si, apruebo — crear Orden de Servicio y comenzar",
            "  No, rechazo — no se realiza el trabajo",
            "  Cancelar");
        if (decision.Contains("Cancelar")) return;

        bool aprobado = decision.Contains("apruebo");
        string? obs = aprobado ? null : AskRequired("Motivo del rechazo:");

        MiniOrdenModel? result = null;
        await WithSpinner(aprobado ? "Aprobando y generando Orden de Servicio" : "Procesando rechazo", async () =>
        {
            result = await Api.AprobarRechazarClienteAsync(pres.Id, aprobado,
                string.IsNullOrWhiteSpace(obs) ? null : obs);
        });

        if (result != null)
        {
            if (aprobado)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Panel(
                    $"[grey]Presupuesto:[/]  [cyan]{Markup.Escape(result.NumeroMiniOrden)}[/]\n" +
                    $"[grey]Estado:[/]       [{result.EstadoColor}]{Markup.Escape(result.EstadoNombre ?? "")}[/]\n" +
                    $"[grey]OS Generada:[/]  [bold green]{Markup.Escape(result.NumeroOrden ?? "generada")}[/]\n\n" +
                    "[white]El mecanico ya puede comenzar el trabajo.[/]")
                    .Header("[bold green]  Cliente Aprobo — Orden de Servicio Creada[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("green")));
            }
            else
                AnsiConsole.MarkupLine("\n[red]  Cliente rechazo el presupuesto. No se realizara el trabajo.[/]");
        }
        else
            Error("No se pudo procesar.");

        Pause();
    }

    // ── Completar ─────────────────────────────────────────────────────────────

    private async Task CompletarAsync()
    {
        PrintHeader("Presupuestos", "Marcar trabajo como completado");

        PagedData<MiniOrdenModel>? data = null;
        await WithSpinner("Cargando trabajos en proceso", async () =>
        {
            data = await Api.GetMiniOrdenesAsync(estado: 5);
        });

        var pres = await SeleccionarPresupuesto(data, "Selecciona el trabajo a completar:");
        if (pres == null) return;

        MostrarResumen(pres);

        if (!Confirm("Marcar este trabajo como COMPLETADO?")) return;
        var obs = Ask("Observaciones finales (opcional):");

        MiniOrdenModel? result = null;
        await WithSpinner("Completando", async () =>
        {
            result = await Api.CompletarMiniOrdenAsync(pres.Id, string.IsNullOrWhiteSpace(obs) ? null : obs);
        });

        if (result != null)
            Ok($"Trabajo completado. Orden de servicio [{result.NumeroOrden}] lista para facturar.");
        else
            Error("No se pudo completar.");

        Pause();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<MiniOrdenModel?> SeleccionarPresupuesto(PagedData<MiniOrdenModel>? data, string titulo)
    {
        if (data == null || data.Items.Count == 0)
        {
            NoData("No hay presupuestos disponibles con ese estado.");
            Pause();
            return null;
        }

        var opciones = data.Items
            .Select(m => $"{Markup.Escape(m.NumeroMiniOrden)}  ({Markup.Escape(m.EstadoNombre ?? "-")})  — {Markup.Escape(m.Descripcion[..Math.Min(45, m.Descripcion.Length)])}")
            .ToList();
        opciones.Add("Cancelar");

        var sel = Choice(titulo, opciones.ToArray());
        if (sel.Contains("Cancelar")) return null;

        return data.Items[opciones.IndexOf(sel)];
    }

    private static void MostrarResumen(MiniOrdenModel m)
    {
        AnsiConsole.Write(new Panel(
            $"[grey]Estado:[/]       [{m.EstadoColor}]{Markup.Escape(m.EstadoNombre ?? "")}[/]\n" +
            $"[grey]Mecanico:[/]     [cyan]{Markup.Escape(m.MecanicoNombre ?? "-")}[/]\n" +
            $"[grey]Descripcion:[/]  {Markup.Escape(m.Descripcion)}\n" +
            $"[grey]Materiales:[/]   [green]$ {m.TotalMateriales:N2}[/]\n" +
            $"[grey]Mano de obra:[/] [green]$ {m.TotalManoObra:N2}[/]\n" +
            $"[grey]TOTAL:[/]        [bold green]$ {m.Total:N2}[/]" +
            (m.Observaciones != null ? $"\n[grey]Observaciones:[/] {Markup.Escape(m.Observaciones)}" : ""))
            .Header($"[bold cyan]  Presupuesto {Markup.Escape(m.NumeroMiniOrden)}[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("blue")));
        AnsiConsole.WriteLine();
    }
}
