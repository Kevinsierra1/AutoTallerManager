using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AutoTallerManager.ConsoleClient.Models;

namespace AutoTallerManager.ConsoleClient.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private string? _token;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest(email, password));
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var data = doc.RootElement.GetProperty("data");
        var auth = data.Deserialize<AuthResponse>(JsonOptions);
        if (auth != null)
        {
            _token = auth.Token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
        return auth;
    }

    public async Task<DashboardResumen?> GetDashboardAsync()
    {
        var response = await _httpClient.GetAsync("api/dashboard/resumen");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("data").Deserialize<DashboardResumen>(JsonOptions);
    }

    public async Task<PagedResponse<ClienteResumen>?> GetClientesAsync(int page = 1, int size = 10)
    {
        var response = await _httpClient.GetAsync($"api/clientes?pageNumber={page}&pageSize={size}");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("data").Deserialize<PagedResponse<ClienteResumen>>(JsonOptions);
    }

    public async Task<PagedResponse<VehiculoResumen>?> GetVehiculosAsync(int page = 1, int size = 10)
    {
        var response = await _httpClient.GetAsync($"api/vehiculos?pageNumber={page}&pageSize={size}");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("data").Deserialize<PagedResponse<VehiculoResumen>>(JsonOptions);
    }

    public async Task<PagedResponse<OrdenResumen>?> GetOrdenesAsync(int page = 1, int size = 10)
    {
        var response = await _httpClient.GetAsync($"api/ordenes?pageNumber={page}&pageSize={size}");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("data").Deserialize<PagedResponse<OrdenResumen>>(JsonOptions);
    }

    public async Task<PagedResponse<RepuestoResumen>?> GetRepuestosAsync(int page = 1, int size = 10, bool? critico = null)
    {
        var url = $"api/repuestos?pageNumber={page}&pageSize={size}";
        if (critico == true) url += "&stockCritico=true";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("data").Deserialize<PagedResponse<RepuestoResumen>>(JsonOptions);
    }

    public async Task<PagedResponse<FacturaResumen>?> GetFacturasAsync(int page = 1, int size = 10)
    {
        var response = await _httpClient.GetAsync($"api/facturas?pageNumber={page}&pageSize={size}");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("data").Deserialize<PagedResponse<FacturaResumen>>(JsonOptions);
    }
}
