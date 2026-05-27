using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;

namespace Application.UseCase.Catalogos;

// ── DTOs ──────────────────────────────────────────────────────────────────────

public record CatalogoItemDto(Guid Id, string Nombre);
public record ModeloItemDto(Guid Id, string Nombre, Guid MarcaId, string MarcaNombre);
public record ColorItemDto(Guid Id, string Nombre, string? CodigoHex);

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
