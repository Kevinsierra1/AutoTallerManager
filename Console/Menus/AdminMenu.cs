using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public class AdminMenu : BaseMenu
{
    public AdminMenu(ApiService api, AuthResponse user) : base(api, user) { }

    public async Task ShowAsync()
    {
        while (true)
        {
            PrintHeader("Administración", "Gestión de usuarios, roles y seguridad");

            var opcion = Choice("  Selecciona una opción:",
                "  👤 Gestión de Usuarios",
                "  🏷  Gestión de Roles",
                "  🔧 Panel de Mecánicos",
                "  🔒 Seguridad & Auditoría",
                "  ← Volver al Menú Principal");

            if (opcion.Contains("Volver")) return;

            if (opcion.Contains("Usuarios"))  await GestionUsuariosAsync();
            else if (opcion.Contains("Roles")) await GestionRolesAsync();
            else if (opcion.Contains("Mecánicos")) await PanelMecanicosAsync();
            else if (opcion.Contains("Seguridad")) await SeguridadAsync();
        }
    }

    // ── Gestión de Usuarios ───────────────────────────────────────────────────

    private async Task GestionUsuariosAsync()
    {
        while (true)
        {
            PrintHeader("Administración", "Gestión de Usuarios");

            var opcion = Choice("  Selecciona:",
                "  📋 Listar Usuarios",
                "  ➕ Crear Usuario",
                "  🏷  Asignar Rol a Usuario",
                "  🗑  Eliminar Usuario",
                "  ← Volver");

            if (opcion.Contains("Volver")) return;

            if (opcion.Contains("Listar")) await ListarUsuariosAsync();
            else if (opcion.Contains("Crear")) await CrearUsuarioAsync();
            else if (opcion.Contains("Asignar Rol")) await AsignarRolAsync();
            else if (opcion.Contains("Eliminar")) await EliminarUsuarioAsync();
        }
    }

    private async Task ListarUsuariosAsync()
    {
        PrintHeader("Administración", "Listado de Usuarios");

        PagedData<UsuarioModel>? data = null;
        await WithSpinner("Cargando usuarios", async () =>
        {
            data = await Api.GetUsuariosAsync();
        });

        if (data == null || data.Items.Count == 0)
        {
            NoData("No hay usuarios registrados.");
            Pause();
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .Expand();

        tabla.AddColumn("[bold]Nombre[/]");
        tabla.AddColumn("[bold]Email[/]");
        tabla.AddColumn("[bold]Roles[/]");
        tabla.AddColumn(new TableColumn("[bold]Estado[/]").Centered());
        tabla.AddColumn("[bold]Creado[/]");

        foreach (var u in data.Items)
        {
            tabla.AddRow(
                $"[white]{Markup.Escape(u.NombreCompleto)}[/]",
                $"[grey]{Markup.Escape(u.Email)}[/]",
                $"[yellow]{Markup.Escape(u.RolesStr)}[/]",
                u.Activo ? "[green]Activo[/]" : "[red]Inactivo[/]",
                $"[grey]{u.CreadoEn:dd/MM/yy}[/]"
            );
        }

        AnsiConsole.Write(tabla);
        AnsiConsole.MarkupLine($"\n[grey]  Total: {data.TotalCount} usuario(s)[/]");
        Pause();
    }

    private async Task CrearUsuarioAsync()
    {
        PrintHeader("Administración", "Crear Usuario");

        var nombres = AskRequired("Nombres:");
        var apellidos = AskRequired("Apellidos:");
        var email = AskRequired("Email:");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[cyan]  Contraseña:[/]").Secret());

        // Seleccionar rol
        List<RolModel>? roles = null;
        await WithSpinner("Cargando roles", async () =>
        {
            roles = await Api.GetRolesAsync();
        });

        if (roles == null || roles.Count == 0)
        {
            Error("No se pudieron cargar los roles.");
            Pause();
            return;
        }

        var opcionesRol = roles.Select(r => $"{Markup.Escape(r.Nombre)} — {Markup.Escape(r.Descripcion ?? "")}").ToList();
        opcionesRol.Add("Sin rol (asignar después)");

        var rolSel = Choice("Selecciona el rol:", opcionesRol.ToArray());
        var rolId = rolSel.Contains("Sin rol") ? (Guid?)null
            : roles[opcionesRol.IndexOf(rolSel)].Id;

        UsuarioModel? result = null;
        await WithSpinner("Creando usuario", async () =>
        {
            result = await Api.CreateUsuarioAsync(new
            {
                Nombres = nombres,
                Apellidos = apellidos,
                Email = email,
                Password = password,
                RolIds = rolId.HasValue ? new[] { rolId.Value } : Array.Empty<Guid>()
            });
        });

        if (result != null)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Panel(
                $"[white]Nombre:[/]  [cyan]{Markup.Escape(result.NombreCompleto)}[/]\n" +
                $"[white]Email:[/]   [grey]{Markup.Escape(result.Email)}[/]\n" +
                $"[white]Roles:[/]   [yellow]{Markup.Escape(result.RolesStr)}[/]")
                .Header("[bold green]  ✓ Usuario Creado[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("green")));
        }
        else
            Error("Error al crear el usuario. Verifica que el email no esté registrado.");

        Pause();
    }

    private async Task AsignarRolAsync()
    {
        PrintHeader("Administración", "Asignar Rol a Usuario");

        PagedData<UsuarioModel>? usuarios = null;
        List<RolModel>? roles = null;

        await WithSpinner("Cargando datos", async () =>
        {
            usuarios = await Api.GetUsuariosAsync(size: 50);
            roles = await Api.GetRolesAsync();
        });

        if (usuarios == null || roles == null)
        {
            Error("No se pudieron cargar los datos.");
            Pause();
            return;
        }

        var opcionesUsuario = usuarios.Items.Select(u => $"{Markup.Escape(u.NombreCompleto)} ({Markup.Escape(u.Email)}) — {Markup.Escape(u.RolesStr)}").ToList();
        opcionesUsuario.Add("← Cancelar");
        var selUser = Choice("Selecciona el usuario:", opcionesUsuario.ToArray());
        if (selUser.Contains("Cancelar")) return;

        var usuario = usuarios.Items[opcionesUsuario.IndexOf(selUser)];

        var opcionesRol = roles.Select(r => $"{Markup.Escape(r.Nombre)} — {Markup.Escape(r.Descripcion ?? "")}").ToList();
        opcionesRol.Add("← Cancelar");
        var selRol = Choice($"Asignar rol a {Markup.Escape(usuario.NombreCompleto)}:", opcionesRol.ToArray());
        if (selRol.Contains("Cancelar")) return;

        var rol = roles[opcionesRol.IndexOf(selRol)];

        var (ok, error) = (false, "");
        await WithSpinner("Asignando rol", async () =>
        {
            (ok, error) = await Api.AsignarRolAsync(usuario.Id, rol.Id);
        });

        if (ok)
            Ok($"Rol [{rol.Nombre}] asignado a {usuario.NombreCompleto}.");
        else
            Error(error ?? "Error al asignar el rol.");

        Pause();
    }

    private async Task EliminarUsuarioAsync()
    {
        PrintHeader("Administración", "Eliminar Usuario");

        PagedData<UsuarioModel>? usuarios = null;
        await WithSpinner("Cargando usuarios", async () =>
        {
            usuarios = await Api.GetUsuariosAsync(size: 50);
        });

        if (usuarios == null || usuarios.Items.Count == 0)
        {
            NoData();
            Pause();
            return;
        }

        var opciones = usuarios.Items.Select(u => $"{u.NombreCompleto} ({u.Email})").ToList();
        opciones.Add("← Cancelar");
        var sel = Choice("Selecciona el usuario a eliminar:", opciones.ToArray());
        if (sel.Contains("Cancelar")) return;

        var usuario = usuarios.Items[opciones.IndexOf(sel)];

        AnsiConsole.MarkupLine($"\n[red]  ⚠ ¿Eliminar permanentemente a [{Markup.Escape(usuario.NombreCompleto)}] ({Markup.Escape(usuario.Email)})?[/]");
        if (!Confirm("Esta acción no se puede deshacer.")) return;

        var (ok, error) = (false, "");
        await WithSpinner("Eliminando", async () =>
        {
            (ok, error) = await Api.DeleteUsuarioAsync(usuario.Id);
        });

        if (ok)
            Ok($"Usuario {usuario.NombreCompleto} eliminado.");
        else
            Error(error ?? "Error al eliminar.");

        Pause();
    }

    // ── Gestión de Roles ──────────────────────────────────────────────────────

    private async Task GestionRolesAsync()
    {
        PrintHeader("Administración", "Roles del Sistema");

        List<RolModel>? roles = null;
        await WithSpinner("Cargando roles", async () =>
        {
            roles = await Api.GetRolesAsync();
        });

        if (roles == null || roles.Count == 0)
        {
            NoData("No hay roles configurados.");
            Pause();
            return;
        }

        var tabla = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("blue"))
            .Expand();

        tabla.AddColumn("[bold]Rol[/]");
        tabla.AddColumn("[bold]Descripción[/]");

        var colores = new[] { "cyan", "yellow", "green", "magenta", "blue", "white" };
        for (int i = 0; i < roles.Count; i++)
        {
            var c = colores[i % colores.Length];
            tabla.AddRow(
                $"[{c}]{Markup.Escape(roles[i].Nombre)}[/]",
                $"[grey]{Markup.Escape(roles[i].Descripcion ?? "")}[/]");
        }

        AnsiConsole.Write(new Panel(tabla)
            .Header("[bold blue]  Roles Configurados[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("blue")));

        Pause();
    }

    // ── Panel de Mecánicos ────────────────────────────────────────────────────

    private async Task PanelMecanicosAsync()
    {
        PrintHeader("Administración", "Panel de Mecánicos");

        PagedData<EmpleadoModel>? datos = null;
        await WithSpinner("Cargando mecánicos", async () =>
        {
            datos = await Api.GetEmpleadosAsync(size: 100);
        });

        if (datos == null || datos.Items.Count == 0)
        {
            NoData("No hay empleados registrados.");
            Pause();
            return;
        }

        // Solo mostrar tipos mecánicos: 0=Mecánico, 1=Eléctrico, 5=Diagnóstico, 6=Área
        var grupos = new[]
        {
            (Tipos: new[] { 0 },    Icono: "🔧", Label: "Mecánicos Generales",      Color: "cyan"),
            (Tipos: new[] { 1 },    Icono: "⚡", Label: "Técnicos Eléctricos",      Color: "yellow"),
            (Tipos: new[] { 5 },    Icono: "🔍", Label: "Mecánicos de Diagnóstico", Color: "blue"),
            (Tipos: new[] { 6 },    Icono: "🔩", Label: "Mecánicos de Área",        Color: "green"),
        };

        foreach (var g in grupos)
        {
            var lista = datos.Items.Where(e => g.Tipos.Contains(e.TipoEmpleado)).ToList();
            if (lista.Count == 0) continue;

            var tabla = new Table()
                .Border(TableBorder.Rounded)
                .BorderStyle(Style.Parse(g.Color))
                .Expand();

            tabla.AddColumn("[bold]Nombre[/]");
            tabla.AddColumn("[bold]Especialidad[/]");
            tabla.AddColumn(new TableColumn("[bold]Estado[/]").Centered());

            foreach (var e in lista)
            {
                tabla.AddRow(
                    $"[white]{Markup.Escape(e.NombreCompleto)}[/]",
                    string.IsNullOrWhiteSpace(e.Especialidad)
                        ? "[grey]Sin especificar[/]"
                        : $"[{g.Color}]{Markup.Escape(e.Especialidad)}[/]",
                    e.Activo ? "[green]✓ Activo[/]" : "[red]✗ Inactivo[/]"
                );
            }

            AnsiConsole.Write(new Panel(tabla)
                .Header($"[bold {g.Color}]  {g.Icono}  {g.Label}  ({lista.Count})[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse(g.Color)));
            AnsiConsole.WriteLine();
        }

        AnsiConsole.MarkupLine($"[grey]  Total mecánicos: {datos.Items.Count(e => e.TipoEmpleado is 0 or 1 or 5 or 6)}[/]");
        Pause();
    }

    // ── Seguridad & Auditoría ─────────────────────────────────────────────────

    private async Task SeguridadAsync()
    {
        PrintHeader("Administración", "Seguridad & Auditoría");

        AnsiConsole.Write(new Panel(
            "[yellow]  Los módulos de Historial de Accesos, Sesiones Activas\n" +
            "  y Logs de Errores están disponibles en la API:\n\n" +
            "  [white]GET /api/Seguridad/historial-accesos[/]\n" +
            "  [white]GET /api/Seguridad/sesiones[/]\n" +
            "  [white]GET /api/Seguridad/logs-errores[/]\n\n" +
            "  Puedes consultarlos desde Swagger en:\n" +
            "  [white]http://localhost:5000/swagger[/][/]")
            .Header("[bold yellow]  Seguridad[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("yellow")));

        Pause();
    }
}
