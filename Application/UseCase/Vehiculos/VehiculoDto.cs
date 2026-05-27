namespace Application.UseCase.Vehiculos;

public record VehiculoDto(
    Guid Id,
    string Placa,
    string? Vin,
    Guid ModeloVehiculoId,
    string? Marca,
    string? Modelo,
    Guid? ColorId,
    string? Color,
    int Anio,
    string? NumeroMotor,
    string? NumeroChasis,
    int KilometrajeActual,
    string? Observaciones,
    bool Activo,
    DateTime CreadoEn
);

public record CreateVehiculoDto(
    string Placa,
    string? Vin,
    Guid ModeloVehiculoId,
    Guid? ColorId,
    int Anio,
    string? Observaciones
);

public record UpdateVehiculoDto(
    string? Placa,
    Guid? ColorId,
    int? KilometrajeActual,
    string? Observaciones,
    bool? Activo
);

public record VehiculoFiltroDto(
    string? Placa,
    string? Vin,
    Guid? MarcaId,
    bool? Activo,
    int PageNumber = 1,
    int PageSize = 10
);
