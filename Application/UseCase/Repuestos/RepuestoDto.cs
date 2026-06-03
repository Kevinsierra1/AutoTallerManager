namespace Application.UseCase.Repuestos;

public record RepuestoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    Guid CategoriaRepuestoId,
    string? Categoria,
    Guid? TipoServicioId,
    string? TipoServicioNombre,
    decimal PrecioCompra,
    decimal PrecioVenta,
    int StockActual,
    int StockMinimo,
    string? Unidad,
    bool Activo
);

public record CreateRepuestoDto(
    string Codigo,
    string Nombre,
    string? Descripcion,
    Guid CategoriaRepuestoId,
    Guid? TipoServicioId,
    decimal PrecioCompra,
    decimal PrecioVenta,
    int StockActual,
    int StockMinimo,
    string? Unidad
);

public record UpdateRepuestoDto(
    string? Nombre,
    string? Descripcion,
    Guid? TipoServicioId,
    decimal? PrecioCompra,
    decimal? PrecioVenta,
    int? StockMinimo,
    string? Unidad,
    bool? Activo
);

public record RepuestoFiltroDto(
    string? Busqueda,
    Guid? CategoriaId,
    Guid? TipoServicioId,
    bool? StockCritico,
    bool? Activo,
    int PageNumber = 1,
    int PageSize = 10
);
