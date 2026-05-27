using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using Domain.Exceptions;
using Application.Common;

namespace Api.Helpers.Errors;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            NotFoundException notFound => (HttpStatusCode.NotFound, notFound.Message, (List<string>?)null),
            ValidationException validation => (HttpStatusCode.BadRequest, "Errores de validación.", validation.Errors.SelectMany(e => e.Value).ToList()),
            ForbiddenAccessException => (HttpStatusCode.Forbidden, "Acceso denegado.", null),
            StockInsuficienteException stock => (HttpStatusCode.BadRequest, stock.Message, null),
            DomainException domain => (HttpStatusCode.BadRequest, domain.Message, null),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "No autorizado.", null),
            _ => (HttpStatusCode.InternalServerError, "Error interno del servidor.", null)
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(message, errors);
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
