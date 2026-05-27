using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public class ClientesMenu : BaseMenu
{
    public ClientesMenu(ApiService api, AuthResponse user) : base(api, user) { }

    public async Task ShowAsync()
    {
        while (true)
        {
            PrintHeader("Gestión de Clientes");

            var opcion = Choice("  Clientes:",
                "  Listar Clientes",
                "  Buscar Cliente",
                "  Crear Cliente",
                "  Actualizar Cliente",
                "  Eliminar Cliente",
                "  Volver al Menú Principal");

            if (opcion.Contains("Volver")) return;

            if (opcion.Contains("Listar"))   await ListarAsync();
            else if (opcion.Contains("Buscar"))   await BuscarAsync();
            else if (opcion.Contains("Crear"))    await CrearAsync();
            else if (opcion.Contains("Actualizar")) await ActualizarAsync();
            else if (opcion.Contains("Eliminar")) await EliminarAsync();
        }
    }

    // ── Listar ──────────────────────────────────────────────────────────────

    private async Task ListarAsync(string? busqueda = null)
    {
        PrintHeader("Clientes", busqueda != null ? $"Búsqueda: \"{busqueda}\"" : "Todos los clientes");

        PagedData<ClienteModel>? data = null;
        await WithSpinner("Cargando clientes", async () =>
        {
            data = await Api.GetClientesAsync(1, 20, busqueda);
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData(busqueda != null ? "No se encontraron clientes con esa búsqueda." : "No hay clientes registrados.");
            Pause();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .Expand();

        table.AddColumn(new TableColumn("[bold]Nombre[/]"));
        table.AddColumn(new TableColumn("[bold]Documento[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Email[/]"));
        table.AddColumn(new TableColumn("[bold]Teléfono[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Registrado[/]").Centered());

        foreach (var c in data.Items)
        {
            table.AddRow(
                Markup.Escape(c.NombreCompleto),
                $"[grey]{Markup.Escape(c.TipoDocumento)}[/] {Markup.Escape(c.NumeroDocumento)}",
                Markup.Escape(c.Email ?? "-"),
                Markup.Escape(c.Telefono ?? "-"),
                c.CreadoEn.ToString("dd/MM/yyyy")
            );
        }

        AnsiConsole.Write(table);
        Info($"Mostrando {data.Items.Count} de {data.TotalCount} clientes");
        Pause();
    }

    // ── Buscar ──────────────────────────────────────────────────────────────

    private async Task BuscarAsync()
    {
        PrintHeader("Buscar Cliente");
        var busqueda = AskRequired("Ingresa nombre, documento o email:");
        await ListarAsync(busqueda);
    }

    // ── Crear ───────────────────────────────────────────────────────────────

    private async Task CrearAsync()
    {
        PrintHeader("Crear Cliente");

        var nombres = AskRequired("Nombres:");
        var apellidos = AskRequired("Apellidos:");

        var tipoDoc = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Tipo de Documento:[/]")
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices("CC", "CE", "PA", "NIT"));

        var numDoc = AskRequired("Número de Documento:");
        var email  = Ask("Email (opcional):");
        var tel    = Ask("Teléfono (opcional):");

        AnsiConsole.WriteLine();

        if (!Confirm("¿Crear este cliente?")) return;

        ClienteModel? created = null;
        await WithSpinner("Creando cliente", async () =>
        {
            created = await Api.CreateClienteAsync(new
            {
                Nombres = nombres, Apellidos = apellidos,
                TipoDocumento = tipoDoc, NumeroDocumento = numDoc,
                Email = string.IsNullOrEmpty(email) ? null : email,
                Telefono = string.IsNullOrEmpty(tel) ? null : tel
            });
        });

        if (created != null) Ok($"Cliente '{created.NombreCompleto}' creado correctamente.");
        else Error("No se pudo crear el cliente.");
        Pause();
    }

    // ── Actualizar ──────────────────────────────────────────────────────────

    private async Task ActualizarAsync()
    {
        PrintHeader("Actualizar Cliente");

        var busqueda = AskRequired("Busca el cliente (nombre/documento):");
        PagedData<ClienteModel>? data = null;
        await WithSpinner("Buscando", async () => { data = await Api.GetClientesAsync(1, 20, busqueda); });

        if (data == null || data.Items.Count == 0) { NoData(); Pause(); return; }

        var opciones = data.Items.Select(c => $"{c.NombreCompleto} | {c.NumeroDocumento}").ToList();
        opciones.Add("Cancelar");

        var sel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Selecciona el cliente:[/]")
                .PageSize(10)
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices(opciones));

        if (sel == "Cancelar") return;

        var cliente = data.Items[opciones.IndexOf(sel)];

        AnsiConsole.WriteLine();
        Info($"Editando: {cliente.NombreCompleto} — deja vacío para mantener el valor actual.");
        AnsiConsole.WriteLine();

        var nombres   = Ask($"Nombres [{cliente.Nombres}]:", cliente.Nombres);
        var apellidos = Ask($"Apellidos [{cliente.Apellidos}]:", cliente.Apellidos);
        var email     = Ask($"Email [{cliente.Email ?? "vacío"}]:", cliente.Email ?? "");
        var tel       = Ask($"Teléfono [{cliente.Telefono ?? "vacío"}]:", cliente.Telefono ?? "");
        var dir       = Ask($"Dirección [{cliente.Direccion ?? "vacío"}]:", cliente.Direccion ?? "");

        if (!Confirm("¿Guardar cambios?")) return;

        ClienteModel? updated = null;
        await WithSpinner("Actualizando", async () =>
        {
            updated = await Api.UpdateClienteAsync(cliente.Id, new
            {
                Nombres = nombres, Apellidos = apellidos,
                Email = string.IsNullOrEmpty(email) ? null : email,
                Telefono = string.IsNullOrEmpty(tel) ? null : tel,
                Direccion = string.IsNullOrEmpty(dir) ? null : dir
            });
        });

        if (updated != null) Ok("Cliente actualizado correctamente.");
        else Error("No se pudo actualizar el cliente.");
        Pause();
    }

    // ── Eliminar ────────────────────────────────────────────────────────────

    private async Task EliminarAsync()
    {
        PrintHeader("Eliminar Cliente");

        var busqueda = AskRequired("Busca el cliente (nombre/documento):");
        PagedData<ClienteModel>? data = null;
        await WithSpinner("Buscando", async () => { data = await Api.GetClientesAsync(1, 20, busqueda); });

        if (data == null || data.Items.Count == 0) { NoData(); Pause(); return; }

        var opciones = data.Items.Select(c => $"{c.NombreCompleto} | {c.NumeroDocumento}").ToList();
        opciones.Add("Cancelar");

        var sel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Selecciona el cliente a eliminar:[/]")
                .PageSize(10)
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices(opciones));

        if (sel == "Cancelar") return;

        var cliente = data.Items[opciones.IndexOf(sel)];

        if (!Confirm($"¿Eliminar a '{cliente.NombreCompleto}'? Esta acción es irreversible.")) return;

        (bool ok, string? error) result = (false, null);
        await WithSpinner("Eliminando", async () =>
        {
            result = await Api.DeleteClienteAsync(cliente.Id);
        });

        if (result.ok) Ok("Cliente eliminado (soft delete).");
        else Error(result.error ?? "No se pudo eliminar.");
        Pause();
    }
}
