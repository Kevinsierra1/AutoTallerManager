using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;

namespace Application.UseCase.Catalogos;

// ── DTOs ──────────────────────────────────────────────────────────────────────

public record CatalogoItemDto(Guid Id, string Nombre);
public record ModeloItemDto(Guid Id, string Nombre, Guid MarcaId, string MarcaNombre);
public record ColorItemDto(Guid Id, string Nombre, string? CodigoHex);
public record EstadoOrdenItemDto(Guid Id, string Nombre, int Codigo, string? Descripcion);
public record PrioridadOrdenItemDto(Guid Id, string Nombre, int Nivel, string? Descripcion);
public record ConfiguracionItemDto(Guid Id, string Clave, string Valor, string Tipo, string? Grupo, string? Descripcion, bool EsEditable);

// ── Marcas ────────────────────────────────────────────────────────────────────

public record GetMarcasQuery : IRequest<List<CatalogoItemDto>>;

public class GetMarcasQueryHandler : IRequestHandler<GetMarcasQuery, List<CatalogoItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMarcasQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CatalogoItemDto>> Handle(GetMarcasQuery request, CancellationToken cancellationToken) =>
        await _context.Marcas
            .OrderBy(m => m.Nombre)
            .Select(m => new CatalogoItemDto(m.Id, m.Nombre))
            .ToListAsync(cancellationToken);
}

// ── Modelos ───────────────────────────────────────────────────────────────────

public record GetModelosQuery(Guid? MarcaId = null) : IRequest<List<ModeloItemDto>>;

public class GetModelosQueryHandler : IRequestHandler<GetModelosQuery, List<ModeloItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetModelosQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ModeloItemDto>> Handle(GetModelosQuery request, CancellationToken cancellationToken)
    {
        var q = _context.ModelosVehiculo.Include(m => m.Marca).AsQueryable();
        if (request.MarcaId.HasValue)
            q = q.Where(m => m.MarcaId == request.MarcaId.Value);
        return await q
            .OrderBy(m => m.Marca.Nombre).ThenBy(m => m.Nombre)
            .Select(m => new ModeloItemDto(m.Id, m.Nombre, m.MarcaId, m.Marca.Nombre))
            .ToListAsync(cancellationToken);
    }
}

// ── Colores ───────────────────────────────────────────────────────────────────

public record GetColoresQuery : IRequest<List<ColorItemDto>>;

public class GetColoresQueryHandler : IRequestHandler<GetColoresQuery, List<ColorItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetColoresQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ColorItemDto>> Handle(GetColoresQuery request, CancellationToken cancellationToken) =>
        await _context.Colores
            .OrderBy(c => c.Nombre)
            .Select(c => new ColorItemDto(c.Id, c.Nombre, c.CodigoHex))
            .ToListAsync(cancellationToken);
}

// ── Estados de Orden ──────────────────────────────────────────────────────────

public record GetEstadosOrdenQuery : IRequest<List<EstadoOrdenItemDto>>;

public class GetEstadosOrdenQueryHandler : IRequestHandler<GetEstadosOrdenQuery, List<EstadoOrdenItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetEstadosOrdenQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<EstadoOrdenItemDto>> Handle(GetEstadosOrdenQuery request, CancellationToken ct) =>
        await _context.EstadosOrden.Where(e => e.Activo)
            .OrderBy(e => e.Codigo)
            .Select(e => new EstadoOrdenItemDto(e.Id, e.Nombre, e.Codigo, e.Descripcion))
            .ToListAsync(ct);
}

// ── Estados de Cita ───────────────────────────────────────────────────────────

public record GetEstadosCitaQuery : IRequest<List<CatalogoItemDto>>;

public class GetEstadosCitaQueryHandler : IRequestHandler<GetEstadosCitaQuery, List<CatalogoItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetEstadosCitaQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CatalogoItemDto>> Handle(GetEstadosCitaQuery request, CancellationToken ct) =>
        await _context.EstadosCita.Where(e => e.Activo)
            .OrderBy(e => e.Nombre)
            .Select(e => new CatalogoItemDto(e.Id, e.Nombre))
            .ToListAsync(ct);
}

// ── Estados de Factura ────────────────────────────────────────────────────────

public record GetEstadosFacturaQuery : IRequest<List<CatalogoItemDto>>;

public class GetEstadosFacturaQueryHandler : IRequestHandler<GetEstadosFacturaQuery, List<CatalogoItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetEstadosFacturaQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CatalogoItemDto>> Handle(GetEstadosFacturaQuery request, CancellationToken ct) =>
        await _context.EstadosFactura.Where(e => e.Activo)
            .OrderBy(e => e.Nombre)
            .Select(e => new CatalogoItemDto(e.Id, e.Nombre))
            .ToListAsync(ct);
}

// ── Prioridades de Orden ──────────────────────────────────────────────────────

public record GetPrioridadesOrdenQuery : IRequest<List<PrioridadOrdenItemDto>>;

public class GetPrioridadesOrdenQueryHandler : IRequestHandler<GetPrioridadesOrdenQuery, List<PrioridadOrdenItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetPrioridadesOrdenQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<PrioridadOrdenItemDto>> Handle(GetPrioridadesOrdenQuery request, CancellationToken ct) =>
        await _context.PrioridadesOrden.Where(p => p.Activo)
            .OrderBy(p => p.Nivel)
            .Select(p => new PrioridadOrdenItemDto(p.Id, p.Nombre, p.Nivel, p.Descripcion))
            .ToListAsync(ct);
}

// ── Tipos de Combustible ──────────────────────────────────────────────────────

public record GetTiposCombustibleQuery : IRequest<List<CatalogoItemDto>>;

public class GetTiposCombustibleQueryHandler : IRequestHandler<GetTiposCombustibleQuery, List<CatalogoItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetTiposCombustibleQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CatalogoItemDto>> Handle(GetTiposCombustibleQuery request, CancellationToken ct) =>
        await _context.TiposCombustible.Where(t => t.Activo)
            .OrderBy(t => t.Nombre)
            .Select(t => new CatalogoItemDto(t.Id, t.Nombre))
            .ToListAsync(ct);
}

// ── Tipos de Transmisión ──────────────────────────────────────────────────────

public record GetTiposTransmisionQuery : IRequest<List<CatalogoItemDto>>;

public class GetTiposTransmisionQueryHandler : IRequestHandler<GetTiposTransmisionQuery, List<CatalogoItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetTiposTransmisionQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CatalogoItemDto>> Handle(GetTiposTransmisionQuery request, CancellationToken ct) =>
        await _context.TiposTransmision.Where(t => t.Activo)
            .OrderBy(t => t.Nombre)
            .Select(t => new CatalogoItemDto(t.Id, t.Nombre))
            .ToListAsync(ct);
}

// ── Tipos Movimiento Inventario ───────────────────────────────────────────────

public record GetTiposMovInventarioQuery : IRequest<List<CatalogoItemDto>>;

public class GetTiposMovInventarioQueryHandler : IRequestHandler<GetTiposMovInventarioQuery, List<CatalogoItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetTiposMovInventarioQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CatalogoItemDto>> Handle(GetTiposMovInventarioQuery request, CancellationToken ct) =>
        await _context.TiposMovimientoInventario.Where(t => t.Activo)
            .OrderBy(t => t.Nombre)
            .Select(t => new CatalogoItemDto(t.Id, t.Nombre))
            .ToListAsync(ct);
}

// ── Configuraciones del Sistema ───────────────────────────────────────────────

public record GetConfiguracionesQuery(string? Grupo = null) : IRequest<List<ConfiguracionItemDto>>;

public class GetConfiguracionesQueryHandler : IRequestHandler<GetConfiguracionesQuery, List<ConfiguracionItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetConfiguracionesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<ConfiguracionItemDto>> Handle(GetConfiguracionesQuery request, CancellationToken ct)
    {
        var q = _context.ConfiguracionesSistema.Where(c => c.Activo).AsQueryable();
        if (!string.IsNullOrEmpty(request.Grupo))
            q = q.Where(c => c.Grupo == request.Grupo);
        return await q.OrderBy(c => c.Grupo).ThenBy(c => c.Clave)
            .Select(c => new ConfiguracionItemDto(c.Id, c.Clave, c.Valor, c.Tipo, c.Grupo, c.Descripcion, c.Activo))
            .ToListAsync(ct);
    }
}

// ── Tipos de Documento ────────────────────────────────────────────────────────

public record TipoDocumentoItemDto(Guid Id, string Nombre, string? Abreviatura);
public record GetTiposDocumentoQuery : IRequest<List<TipoDocumentoItemDto>>;

public class GetTiposDocumentoQueryHandler : IRequestHandler<GetTiposDocumentoQuery, List<TipoDocumentoItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetTiposDocumentoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<TipoDocumentoItemDto>> Handle(GetTiposDocumentoQuery request, CancellationToken ct) =>
        await _context.TiposDocumento
            .Where(t => !t.Eliminado)
            .OrderBy(t => t.Nombre)
            .Select(t => new TipoDocumentoItemDto(t.Id, t.Nombre, t.Abreviatura))
            .ToListAsync(ct);
}

// ── Tipos de Servicio ─────────────────────────────────────────────────────────

public record TipoServicioItemDto(Guid Id, string Nombre, string? Descripcion, decimal? PrecioBase, bool Activo);
public record GetTiposServicioQuery : IRequest<List<TipoServicioItemDto>>;

public class GetTiposServicioQueryHandler : IRequestHandler<GetTiposServicioQuery, List<TipoServicioItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetTiposServicioQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<TipoServicioItemDto>> Handle(GetTiposServicioQuery request, CancellationToken ct) =>
        await _context.TiposServicio
            .Where(t => !t.Eliminado)
            .OrderBy(t => t.Nombre)
            .Select(t => new TipoServicioItemDto(t.Id, t.Nombre, t.Descripcion, t.PrecioBase, t.Activo))
            .ToListAsync(ct);
}

// ── Métodos de Pago ───────────────────────────────────────────────────────────

public record GetMetodosPagoQuery : IRequest<List<CatalogoItemDto>>;

public class GetMetodosPagoQueryHandler : IRequestHandler<GetMetodosPagoQuery, List<CatalogoItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMetodosPagoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CatalogoItemDto>> Handle(GetMetodosPagoQuery request, CancellationToken ct) =>
        await _context.MetodosPago
            .Where(m => m.Activo && !m.Eliminado)
            .OrderBy(m => m.Nombre)
            .Select(m => new CatalogoItemDto(m.Id, m.Nombre))
            .ToListAsync(ct);
}

// ── Categorías de Repuesto ────────────────────────────────────────────────────

public record CategoriaRepuestoItemDto(Guid Id, string Nombre, string? Descripcion);
public record GetCategoriasRepuestoQuery : IRequest<List<CategoriaRepuestoItemDto>>;

public class GetCategoriasRepuestoQueryHandler : IRequestHandler<GetCategoriasRepuestoQuery, List<CategoriaRepuestoItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetCategoriasRepuestoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CategoriaRepuestoItemDto>> Handle(GetCategoriasRepuestoQuery request, CancellationToken ct) =>
        await _context.CategoriasRepuesto
            .Where(c => !c.Eliminado)
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaRepuestoItemDto(c.Id, c.Nombre, c.Descripcion))
            .ToListAsync(ct);
}

// ── Permisos ──────────────────────────────────────────────────────────────────

public record PermisoItemDto(Guid Id, string Nombre, string Clave, string? Modulo);
public record GetPermisosQuery(string? Modulo = null) : IRequest<List<PermisoItemDto>>;

public class GetPermisosQueryHandler : IRequestHandler<GetPermisosQuery, List<PermisoItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetPermisosQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<PermisoItemDto>> Handle(GetPermisosQuery request, CancellationToken ct)
    {
        var q = _context.Permisos.Where(p => p.Activo).AsQueryable();
        if (!string.IsNullOrEmpty(request.Modulo))
            q = q.Where(p => p.Modulo == request.Modulo);
        return await q.OrderBy(p => p.Modulo).ThenBy(p => p.Nombre)
            .Select(p => new PermisoItemDto(p.Id, p.Nombre, p.Clave, p.Modulo))
            .ToListAsync(ct);
    }
}

public record UpdateConfiguracionDto(string Valor);
