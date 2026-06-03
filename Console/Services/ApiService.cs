using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AutoTaller.Console.Models;

namespace AutoTaller.Console.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public string? CurrentToken { get; private set; }
    public bool IsAuthenticated => CurrentToken != null;

    public ApiService(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(30) };
    }

    public void SetToken(string token)
    {
        CurrentToken = token;
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearToken()
    {
        CurrentToken = null;
        _http.DefaultRequestHeaders.Authorization = null;
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            var resp = await _http.GetAsync(url);
            var body = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResult<T>>(body, _json);
            return result is { Exito: true } ? result.Data : default;
        }
        catch { return default; }
    }

    private async Task<(bool ok, string? error)> PostAsync(string url, object payload)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync(url, content);
            if (resp.StatusCode == System.Net.HttpStatusCode.NoContent) return (true, null);
            var body = await resp.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body)) return (resp.IsSuccessStatusCode ? (true, null) : (false, "Error del servidor"));
            var result = JsonSerializer.Deserialize<ApiResult<object>>(body, _json);
            if (result?.Exito == true) return (true, null);
            var err = result?.Mensaje ?? result?.Errores?.FirstOrDefault() ?? "Error desconocido";
            return (false, err);
        }
        catch { return (false, "Error de conexión"); }
    }

    private async Task<T?> PostAndGetAsync<T>(string url, object payload)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync(url, content);
            var body = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResult<T>>(body, _json);
            return result is { Exito: true } ? result.Data : default;
        }
        catch { return default; }
    }

    private async Task<T?> PutAndGetAsync<T>(string url, object payload)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _http.PutAsync(url, content);
            var body = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResult<T>>(body, _json);
            return result is { Exito: true } ? result.Data : default;
        }
        catch { return default; }
    }

    private async Task<(bool ok, string? error)> DeleteAsync(string url)
    {
        try
        {
            var resp = await _http.DeleteAsync(url);
            return resp.IsSuccessStatusCode ? (true, null) : (false, $"HTTP {(int)resp.StatusCode}");
        }
        catch { return (false, "Error de conexión"); }
    }

    // ─── Auth ─────────────────────────────────────────────────────────────────

    public async Task<(AuthResponse? data, string? error)> LoginAsync(string email, string password)
    {
        try
        {
            var payload = new { Email = email, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("/api/Auth/login", content);
            var body = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResult<AuthResponse>>(body, _json);
            if (result?.Exito == true && result.Data != null) return (result.Data, null);
            return (null, result?.Mensaje ?? "Credenciales inválidas");
        }
        catch (Exception ex) { return (null, $"Error de conexión: {ex.Message}"); }
    }

    public async Task<(AuthResponse? data, string? error)> RegisterClienteAsync(object dto)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("/api/Auth/register-cliente", content);
            var body = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResult<AuthResponse>>(body, _json);
            if (result?.Exito == true && result.Data != null) return (result.Data, null);
            return (null, result?.Mensaje ?? result?.Errores?.FirstOrDefault() ?? "Error al registrar");
        }
        catch (Exception ex) { return (null, $"Error de conexión: {ex.Message}"); }
    }

    // ─── Clientes ─────────────────────────────────────────────────────────────

    public Task<PagedData<ClienteModel>?> GetClientesAsync(int page = 1, int size = 10, string? busqueda = null)
    {
        var url = $"/api/Clientes?PageNumber={page}&PageSize={size}";
        if (!string.IsNullOrEmpty(busqueda)) url += $"&Busqueda={Uri.EscapeDataString(busqueda)}";
        return GetAsync<PagedData<ClienteModel>>(url);
    }

    public Task<ClienteModel?> GetClienteByIdAsync(Guid id) =>
        GetAsync<ClienteModel>($"/api/Clientes/{id}");

    public Task<ClienteModel?> GetClienteByNumeroAsync(int numero) =>
        GetAsync<ClienteModel>($"/api/Clientes/numero/{numero}");

    public Task<ClienteModel?> CreateClienteAsync(object dto) =>
        PostAndGetAsync<ClienteModel>("/api/Clientes", dto);

    public Task<ClienteModel?> UpdateClienteAsync(Guid id, object dto) =>
        PutAndGetAsync<ClienteModel>($"/api/Clientes/{id}", dto);

    public Task<(bool ok, string? error)> DeleteClienteAsync(Guid id) =>
        DeleteAsync($"/api/Clientes/{id}");

    // ─── Vehículos ────────────────────────────────────────────────────────────

    public Task<PagedData<VehiculoModel>?> GetVehiculosAsync(int page = 1, int size = 10, string? placa = null)
    {
        var url = $"/api/Vehiculos?PageNumber={page}&PageSize={size}";
        if (!string.IsNullOrEmpty(placa)) url += $"&Placa={Uri.EscapeDataString(placa)}";
        return GetAsync<PagedData<VehiculoModel>>(url);
    }

    public Task<VehiculoModel?> CreateVehiculoAsync(object dto) =>
        PostAndGetAsync<VehiculoModel>("/api/Vehiculos", dto);

    // ─── Catalogos ────────────────────────────────────────────────────────────

    public Task<PagedData<object>?> GetMarcasRawAsync() =>
        GetAsync<PagedData<object>>("/api/Vehiculos?PageNumber=1&PageSize=1"); // no hay endpoint catalogo — usamos los de la BD via seed

    // Los catálogos (marcas, modelos, colores, categorías) no tienen endpoints propios expuestos.
    // Los obtenemos del swagger o podemos listar desde la DB. Por ahora usamos listas conocidas del seed.

    // ─── Catálogos ────────────────────────────────────────────────────────────

    public Task<List<CatalogoItem>?> GetMarcasAsync() =>
        GetAsync<List<CatalogoItem>>("/api/Catalogos/marcas");

    public Task<List<ModeloItem>?> GetModelosAsync(Guid? marcaId = null)
    {
        var url = "/api/Catalogos/modelos";
        if (marcaId.HasValue) url += $"?marcaId={marcaId}";
        return GetAsync<List<ModeloItem>>(url);
    }

    public Task<List<ColorItem>?> GetColoresAsync() =>
        GetAsync<List<ColorItem>>("/api/Catalogos/colores");

    // ─── Órdenes ──────────────────────────────────────────────────────────────

    public Task<PagedData<OrdenModel>?> GetOrdenesAsync(int page = 1, int size = 10, int? estado = null)
    {
        var url = $"/api/Ordenes?PageNumber={page}&PageSize={size}";
        if (estado.HasValue) url += $"&Estado={estado}";
        return GetAsync<PagedData<OrdenModel>>(url);
    }

    public Task<OrdenModel?> GetOrdenByIdAsync(Guid id) =>
        GetAsync<OrdenModel>($"/api/Ordenes/{id}");

    public Task<OrdenModel?> CreateOrdenAsync(object dto) =>
        PostAndGetAsync<OrdenModel>("/api/Ordenes", dto);

    public Task<(bool ok, string? error)> AprobarOrdenAsync(Guid id, Guid clienteId) =>
        PostAsync($"/api/Ordenes/{id}/aprobar", clienteId);

    public Task<(bool ok, string? error)> AsignarMecanicoAsync(Guid id, Guid empleadoId) =>
        PostAsync($"/api/Ordenes/{id}/asignar-mecanico", empleadoId);

    public Task<(bool ok, string? error)> FinalizarOrdenAsync(Guid id) =>
        PostAsync($"/api/Ordenes/{id}/finalizar", new { });

    public Task<(bool ok, string? error)> CancelarOrdenAsync(Guid id, string motivo) =>
        PostAsync($"/api/Ordenes/{id}/cancelar", motivo);

    // ─── Repuestos ────────────────────────────────────────────────────────────

    public Task<PagedData<RepuestoModel>?> GetRepuestosAsync(int page = 1, int size = 10, string? busqueda = null, bool? stockCritico = null)
    {
        var url = $"/api/Repuestos?PageNumber={page}&PageSize={size}";
        if (!string.IsNullOrEmpty(busqueda)) url += $"&Busqueda={Uri.EscapeDataString(busqueda)}";
        if (stockCritico.HasValue) url += $"&StockCritico={stockCritico.Value.ToString().ToLower()}";
        return GetAsync<PagedData<RepuestoModel>>(url);
    }

    public Task<RepuestoModel?> GetRepuestoByIdAsync(Guid id) =>
        GetAsync<RepuestoModel>($"/api/Repuestos/{id}");

    public Task<RepuestoModel?> CreateRepuestoAsync(object dto) =>
        PostAndGetAsync<RepuestoModel>("/api/Repuestos", dto);

    public Task<RepuestoModel?> UpdateRepuestoAsync(Guid id, object dto) =>
        PutAndGetAsync<RepuestoModel>($"/api/Repuestos/{id}", dto);

    public Task<(bool ok, string? error)> DeleteRepuestoAsync(Guid id) =>
        DeleteAsync($"/api/Repuestos/{id}");

    // ─── Inventario ───────────────────────────────────────────────────────────

    public Task<PagedData<MovimientoModel>?> GetMovimientosAsync(int page = 1, int size = 10, Guid? repuestoId = null)
    {
        var url = $"/api/Inventario/movimientos?PageNumber={page}&PageSize={size}";
        if (repuestoId.HasValue) url += $"&repuestoId={repuestoId}";
        return GetAsync<PagedData<MovimientoModel>>(url);
    }

    public Task<(bool ok, string? error)> EntradaInventarioAsync(Guid repuestoId, int cantidad, string? motivo) =>
        PostAsync("/api/Inventario/entrada", new { RepuestoId = repuestoId, Cantidad = cantidad, Motivo = motivo });

    public Task<(bool ok, string? error)> SalidaInventarioAsync(Guid repuestoId, int cantidad, string? motivo) =>
        PostAsync("/api/Inventario/salida", new { RepuestoId = repuestoId, Cantidad = cantidad, Motivo = motivo });

    // ─── Facturas ─────────────────────────────────────────────────────────────

    public Task<PagedData<FacturaModel>?> GetFacturasAsync(int page = 1, int size = 10) =>
        GetAsync<PagedData<FacturaModel>>($"/api/Facturas?PageNumber={page}&PageSize={size}");

    public Task<FacturaModel?> GetFacturaByIdAsync(Guid id) =>
        GetAsync<FacturaModel>($"/api/Facturas/{id}");

    public Task<FacturaModel?> GenerarFacturaAsync(Guid ordenId, decimal descuento = 0) =>
        PostAndGetAsync<FacturaModel>("/api/Facturas/generar", new { OrdenServicioId = ordenId, Descuento = descuento });

    // ─── Empleados ────────────────────────────────────────────────────────────

    public Task<PagedData<EmpleadoModel>?> GetEmpleadosAsync(int page = 1, int size = 50) =>
        GetAsync<PagedData<EmpleadoModel>>($"/api/Empleados?PageNumber={page}&PageSize={size}");

    public Task<PagedData<EmpleadoModel>?> GetEmpleadosByTipoAsync(int tipo, int size = 100) =>
        GetAsync<PagedData<EmpleadoModel>>($"/api/Empleados?tipo={tipo}&PageNumber=1&PageSize={size}");

    public Task<EmpleadoModel?> CreateEmpleadoAsync(object dto) =>
        PostAndGetAsync<EmpleadoModel>("/api/Empleados", dto);

    public Task<EmpleadoModel?> UpdateEmpleadoAsync(Guid id, object dto) =>
        PutAndGetAsync<EmpleadoModel>($"/api/Empleados/{id}", dto);

    public Task<(bool ok, string? error)> DeleteEmpleadoAsync(Guid id) =>
        DeleteAsync($"/api/Empleados/{id}");

    // ─── Dashboard ────────────────────────────────────────────────────────────

    public Task<DashboardResumen?> GetDashboardResumenAsync() =>
        GetAsync<DashboardResumen>("/api/Dashboard/resumen");

    public Task<List<OrdenEstadistica>?> GetDashboardOrdenesAsync() =>
        GetAsync<List<OrdenEstadistica>>("/api/Dashboard/ordenes-por-estado");

    public Task<List<FacturacionMensual>?> GetDashboardFacturacionAsync() =>
        GetAsync<List<FacturacionMensual>>("/api/Dashboard/facturacion-mensual");

    public Task<PagedData<RepuestoModel>?> GetRepuestosCriticosAsync(int page = 1, int size = 10) =>
        GetAsync<PagedData<RepuestoModel>>($"/api/Dashboard/repuestos-criticos?pageNumber={page}&pageSize={size}");

    // ─── Mini-Órdenes ─────────────────────────────────────────────────────────

    public Task<PagedData<MiniOrdenModel>?> GetMiniOrdenesAsync(int page = 1, int size = 10, int? estado = null, Guid? mecanicoId = null)
    {
        var url = $"/api/MiniOrdenes?filtro.pageNumber={page}&filtro.pageSize={size}";
        if (estado.HasValue) url += $"&filtro.estado={estado}";
        if (mecanicoId.HasValue) url += $"&filtro.mecanicoId={mecanicoId}";
        return GetAsync<PagedData<MiniOrdenModel>>(url);
    }

    public Task<MiniOrdenModel?> GetMiniOrdenByIdAsync(Guid id) =>
        GetAsync<MiniOrdenModel>($"/api/MiniOrdenes/{id}");

    public Task<MiniOrdenModel?> CreatePresupuestoAsync(Guid clienteId, Guid vehiculoId, string descripcion, string? observaciones, List<object>? detalles) =>
        PostAndGetAsync<MiniOrdenModel>("/api/MiniOrdenes", new
        {
            ClienteId = clienteId,
            VehiculoId = vehiculoId,
            Descripcion = descripcion,
            Observaciones = observaciones,
            Detalles = detalles ?? new List<object>(),
            ManosObra = (object?)null
        });

    public Task<MiniOrdenModel?> EnviarRevisionJefeAsync(Guid id) =>
        PostAndGetAsync<MiniOrdenModel>($"/api/MiniOrdenes/{id}/enviar-revision", new { });

    public Task<MiniOrdenModel?> AprobarRechazarJefeAsync(Guid id, bool aprobado, string? observacion) =>
        PostAndGetAsync<MiniOrdenModel>($"/api/MiniOrdenes/{id}/aprobacion-jefe",
            new { Aprobado = aprobado, Observacion = observacion });

    public Task<MiniOrdenModel?> AprobarRechazarClienteAsync(Guid id, bool aprobado, string? observacion) =>
        PostAndGetAsync<MiniOrdenModel>($"/api/MiniOrdenes/{id}/aprobacion-cliente",
            new { Aprobado = aprobado, Observacion = observacion });

    public Task<MiniOrdenModel?> CompletarMiniOrdenAsync(Guid id, string? observacion = null) =>
        PostAndGetAsync<MiniOrdenModel>($"/api/MiniOrdenes/{id}/completar?observacion={Uri.EscapeDataString(observacion ?? "")}", new { });

    // ─── Usuarios ─────────────────────────────────────────────────────────────

    public Task<PagedData<UsuarioModel>?> GetUsuariosAsync(int page = 1, int size = 20) =>
        GetAsync<PagedData<UsuarioModel>>($"/api/Usuarios?pageNumber={page}&pageSize={size}");

    public Task<UsuarioModel?> CreateUsuarioAsync(object dto) =>
        PostAndGetAsync<UsuarioModel>("/api/Usuarios", dto);

    public Task<(bool ok, string? error)> AsignarRolAsync(Guid usuarioId, Guid rolId) =>
        PostAsync($"/api/Usuarios/{usuarioId}/roles/{rolId}", new { });

    public Task<(bool ok, string? error)> DeleteUsuarioAsync(Guid id) =>
        DeleteAsync($"/api/Usuarios/{id}");

    // ─── Roles ────────────────────────────────────────────────────────────────

    public Task<List<RolModel>?> GetRolesAsync() =>
        GetAsync<List<RolModel>>("/api/Roles");

    // ─── Health check ─────────────────────────────────────────────────────────

    public async Task<bool> PingAsync()
    {
        try
        {
            var resp = await _http.GetAsync("/api/Auth/login"); // any endpoint just to test connection
            return true;
        }
        catch { return false; }
    }
}
