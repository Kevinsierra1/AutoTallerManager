namespace AutoTallerManager.Shared.Models;

public class ApiResponse<T>
{
    public bool Exito { get; init; }
    public string? Mensaje { get; init; }
    public T? Data { get; init; }
    public List<string>? Errores { get; init; }

    private ApiResponse() { }

    public static ApiResponse<T> Success(T data, string? mensaje = null) =>
        new() { Exito = true, Data = data, Mensaje = mensaje };

    public static ApiResponse<T> Fail(string mensaje, List<string>? errores = null) =>
        new() { Exito = false, Mensaje = mensaje, Errores = errores };
}
