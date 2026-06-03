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

        table.AddColumn(new TableColumn("[bold]#[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Nombre[/]"));
        table.AddColumn(new TableColumn("[bold]Documento[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Email[/]"));
        table.AddColumn(new TableColumn("[bold]Teléfono[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Registrado[/]").Centered());

        foreach (var c in data.Items)
        {
            table.AddRow(
                $"[bold cyan]{c.Numero}[/]",
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
        PrintHeader("Crear Cliente", "El cliente recibirá acceso para aprobar sus órdenes");

        var nombres   = AskRequired("Nombres:");
        var apellidos = AskRequired("Apellidos:");

        var tipoDoc = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Tipo de Documento:[/]")
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices("CC", "CE", "PA", "NIT"));

        var numDoc = AskRequired("Número de Documento:");
        var email  = AskRequired("Email (para iniciar sesión):");
        var tel    = Ask("Teléfono (opcional):");

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]  El cliente usará esta contraseña para aprobar sus órdenes de servicio.[/]");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan]  Contraseña:[/]")
                .Secret()
                .Validate(p => p.Length >= 6
                    ? ValidationResult.Success()
                    : ValidationResult.Error("Mínimo 6 caracteres")));

        AnsiConsole.WriteLine();
        if (!Confirm("¿Registrar cliente con acceso al sistema?")) return;

        // Paso 1: crear perfil de cliente
        ClienteModel? created = null;
        await WithSpinner("Creando perfil", async () =>
        {
            created = await Api.CreateClienteAsync(new
            {
                Nombres = nombres, Apellidos = apellidos,
                TipoDocumento = tipoDoc, NumeroDocumento = numDoc,
                Email = email,
                Telefono = string.IsNullOrEmpty(tel) ? null : tel
            });
        });

        if (created == null)
        {
            Error("No se pudo crear el perfil del cliente. Verifica que el documento no esté registrado.");
            Pause();
            return;
        }

        // Paso 2: crear usuario con rol Cliente
        UsuarioModel? usuario = null;
        await WithSpinner("Creando acceso al sistema", async () =>
        {
            var roles = await Api.GetRolesAsync();
            var rolCliente = roles?.FirstOrDefault(r => r.Nombre == "Cliente");
            usuario = await Api.CreateUsuarioAsync(new
            {
                Nombres    = nombres,
                Apellidos  = apellidos,
                Email      = email,
                Password   = password,
                RolIds     = rolCliente != null ? new[] { rolCliente.Id } : Array.Empty<Guid>()
            });
        });

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Panel(
            $"[white]Cliente:[/]    [cyan]{Markup.Escape(created.NombreCompleto)}[/]  [grey](#{created.Numero})[/]\n" +
            $"[white]Documento:[/]  [grey]{Markup.Escape(tipoDoc)} {Markup.Escape(numDoc)}[/]\n" +
            $"[white]Email:[/]      [grey]{Markup.Escape(email)}[/]\n" +
            (usuario != null
                ? "[green]Acceso al sistema creado.[/] El cliente puede iniciar sesión para aprobar sus órdenes."
                : "[yellow]Perfil creado, pero no se pudo crear el acceso.[/] Regístralo en Administración → Gestión de Usuarios."))
            .Header(usuario != null
                ? "[bold green]  ✓ Cliente Registrado[/]"
                : "[bold yellow]  ⚠ Cliente Registrado (sin acceso)[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse(usuario != null ? "green" : "yellow")));

        Pause();
    }

    // ── Actualizar ──────────────────────────────────────────────────────────

    private async Task ActualizarAsync()
    {
        PrintHeader("Actualizar Cliente");

        var cliente = await SeleccionarClienteAsync();
        if (cliente == null) return;

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

        var cliente = await SeleccionarClienteAsync();
        if (cliente == null) return;

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

    // ── Helper: seleccionar cliente ─────────────────────────────────────────
    // Acepta: número directo (5), con # (#5), nombre, documento o email.
    // Si el input es numérico va directo al cliente por #N sin mostrar lista.

    private async Task<ClienteModel?> SeleccionarClienteAsync()
    {
        AnsiConsole.MarkupLine("[grey]  Puedes escribir el [bold]#número[/] (ej: 5 o #5), nombre, documento o email.[/]");
        var input = AskRequired("Busca el cliente:");

        // Búsqueda exacta por número
        var limpio = input.TrimStart('#');
        if (int.TryParse(limpio, out var numero))
        {
            ClienteModel? exacto = null;
            await WithSpinner($"Buscando cliente #{numero}", async () =>
            {
                exacto = await Api.GetClienteByNumeroAsync(numero);
            });

            if (exacto != null)
            {
                AnsiConsole.MarkupLine($"[grey]  Cliente encontrado:[/] [bold cyan]#{exacto.Numero}[/] {Markup.Escape(exacto.NombreCompleto)}");
                return exacto;
            }

            Warn($"No existe ningún cliente con el número #{numero}.");
            Pause();
            return null;
        }

        // Búsqueda por texto → lista de resultados
        PagedData<ClienteModel>? data = null;
        await WithSpinner("Buscando", async () => { data = await Api.GetClientesAsync(1, 20, input); });

        if (data == null || data.Items.Count == 0) { NoData("No se encontraron clientes."); Pause(); return null; }

        // Si hay un solo resultado, lo devuelve directamente
        if (data.Items.Count == 1)
        {
            var unico = data.Items[0];
            AnsiConsole.MarkupLine($"[grey]  Cliente encontrado:[/] [bold cyan]#{unico.Numero}[/] {Markup.Escape(unico.NombreCompleto)}");
            return unico;
        }

        var opciones = data.Items.Select(c => $"#{c.Numero}  {c.NombreCompleto} | {c.NumeroDocumento}").ToList();
        opciones.Add("Cancelar");

        var sel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]  Selecciona el cliente:[/]")
                .PageSize(12)
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices(opciones));

        if (sel == "Cancelar") return null;
        return data.Items[opciones.IndexOf(sel)];
    }
}
