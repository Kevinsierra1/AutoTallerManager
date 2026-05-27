using AutoTallerManager.Domain.Enums;

namespace AutoTallerManager.Application.Features.Citas.DTOs;

public record CitaDto(
    Guid Id,
    Guid ClienteId,
    string? ClienteNombre,
    Guid VehiculoId,
    string? VehiculoPlaca,
    DateTime FechaHora,
    string Motivo,
    EstadoCitaEnum Estado,
    string? Observaciones,
    DateTime CreadoEn
);

public record CreateCitaDto(
    Guid ClienteId,
    Guid VehiculoId,
    DateTime FechaHora,
    string Motivo,
    string? Observaciones
);

public record UpdateCitaDto(
    DateTime? FechaHora,
    string? Motivo,
    EstadoCitaEnum? Estado,
    string? Observaciones
);

public record CitaFiltroDto(
    Guid? ClienteId,
    Guid? VehiculoId,
    EstadoCitaEnum? Estado,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    int PageNumber = 1,
    int PageSize = 10
);
