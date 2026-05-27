using AutoTaller.Console.Models;
using AutoTaller.Console.Services;
using Spectre.Console;

namespace AutoTaller.Console.Menus;

public abstract class BaseMenu
{
    protected readonly ApiService Api;
    protected readonly AuthResponse User;

    protected BaseMenu(ApiService api, AuthResponse user)
    {
        Api = api;
        User = user;
    }

    protected void PrintHeader(string titulo, string? subtitulo = null)
    {
        try { AnsiConsole.Clear(); } catch { }
        AnsiConsole.Write(new Rule($"[bold blue]AutoTaller Manager[/]  [grey]|[/]  [white]{titulo}[/]").RuleStyle("blue"));
        AnsiConsole.MarkupLine($"[grey]  Usuario:[/] [cyan]{User.NombreCompleto}[/]  [grey]Rol:[/] [yellow]{User.RolesStr}[/]");
        if (subtitulo != null)
            AnsiConsole.MarkupLine($"[grey]  {subtitulo}[/]");
        AnsiConsole.WriteLine();
    }

    protected static void Ok(string msg) =>
        AnsiConsole.MarkupLine($"[green]  ✓ {Markup.Escape(msg)}[/]");

    protected static void Error(string msg) =>
        AnsiConsole.MarkupLine($"[red]  ✗ {Markup.Escape(msg)}[/]");

    protected static void Info(string msg) =>
        AnsiConsole.MarkupLine($"[grey]  {Markup.Escape(msg)}[/]");

    protected static void Warn(string msg) =>
        AnsiConsole.MarkupLine($"[yellow]  ⚠ {Markup.Escape(msg)}[/]");

    protected static void Pause()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[grey]  Presiona [white]Enter[/] para continuar...[/]");
        System.Console.ReadLine();
    }

    protected static string Ask(string prompt, string? defaultValue = null)
    {
        var p = new TextPrompt<string>($"[cyan]  {Markup.Escape(prompt)}[/]").AllowEmpty();
        if (defaultValue != null) p.DefaultValue(defaultValue);
        return AnsiConsole.Prompt(p);
    }

    protected static string AskRequired(string prompt)
    {
        return AnsiConsole.Prompt(new TextPrompt<string>($"[cyan]  {Markup.Escape(prompt)}[/]"));
    }

    protected static int AskInt(string prompt, int defaultValue = 0)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<int>($"[cyan]  {Markup.Escape(prompt)}[/]")
                .DefaultValue(defaultValue));
    }

    protected static decimal AskDecimal(string prompt, decimal defaultValue = 0)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<decimal>($"[cyan]  {Markup.Escape(prompt)}[/]")
                .DefaultValue(defaultValue));
    }

    protected static bool Confirm(string prompt) =>
        AnsiConsole.Confirm($"[yellow]  {Markup.Escape(prompt)}[/]", false);

    protected static string Choice(string title, params string[] choices)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[bold]{title}[/]")
                .PageSize(12)
                .HighlightStyle(new Style(Color.Cyan1))
                .AddChoices(choices));
    }

    protected static void NoData(string msg = "Sin registros encontrados")
    {
        AnsiConsole.MarkupLine($"[grey]  (!) {msg}[/]");
    }

    protected static async Task WithSpinner(string msg, Func<Task> action)
    {
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("cyan"))
            .StartAsync($"[cyan]{msg}...[/]", async ctx =>
            {
                await action();
                ctx.Status("[green]Listo[/]");
            });
    }
}
