using Serilog;
using Application;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Data.Seeders;
using Api.Helpers.Errors;
using Api.Extensions;
using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Serilog — toda la configuración viene de appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Application and Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Auth
builder.Services.AddJwtAuthentication(builder.Configuration);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Auto-migrate and seed
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error durante la inicialización de la base de datos.");
    }
}

// Middleware pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();

// Swagger siempre disponible (desactivar en producción real cambiando la condición)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoTallerManager API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "AutoTallerManager API";
    c.DefaultModelsExpandDepth(-1);
});

app.UseIpRateLimiting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Abrir Swagger automáticamente al arrancar en Development
if (app.Environment.IsDevelopment())
{
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var url = "http://localhost:5000/swagger";
        Log.Information("AutoTallerManager API corriendo en {Url}", url);
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch { /* si no puede abrir el browser, continúa normal */ }
    });
}

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
