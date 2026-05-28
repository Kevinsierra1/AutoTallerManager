using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            // Políticas base
            options.AddPolicy("AdminOnly",      policy => policy.RequireRole("Admin"));
            options.AddPolicy("StaffOnly",      policy => policy.RequireRole("Admin", "Mecánico", "Recepcionista", "JefeTaller", "MecanicoDiagnostico", "MecanicoArea"));

            // Flujo M-J-C
            options.AddPolicy("MecanicoOJefe",  policy => policy.RequireRole("Admin", "JefeTaller", "Mecánico", "MecanicoDiagnostico", "MecanicoArea"));
            options.AddPolicy("JefeTallerOnly", policy => policy.RequireRole("Admin", "JefeTaller"));
            options.AddPolicy("MecanicoOnly",   policy => policy.RequireRole("Admin", "JefeTaller", "Mecánico", "MecanicoDiagnostico", "MecanicoArea"));

            // Inventario
            options.AddPolicy("AlmacenOnly",    policy => policy.RequireRole("Admin", "JefeAlmacen", "JefeBodega"));
            options.AddPolicy("AlmacenOTaller", policy => policy.RequireRole("Admin", "JefeTaller", "JefeAlmacen", "JefeBodega", "Mecánico", "MecanicoArea"));

            // Cliente puede aprobar sus propias mini-órdenes
            options.AddPolicy("ClienteOrAdmin", policy => policy.RequireRole("Admin", "Cliente", "Recepcionista"));

            // Recepción y atención al cliente
            options.AddPolicy("RecepcionOnly",  policy => policy.RequireRole("Admin", "Recepcionista", "JefeTaller"));

            // Dashboard y reportes
            options.AddPolicy("Reportes",       policy => policy.RequireRole("Admin", "JefeTaller", "JefeAlmacen", "JefeBodega"));
        });

        return services;
    }
}
