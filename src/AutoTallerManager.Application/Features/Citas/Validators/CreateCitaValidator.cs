using FluentValidation;
using AutoTallerManager.Application.Features.Citas.DTOs;

namespace AutoTallerManager.Application.Features.Citas.Validators;

public class CreateCitaValidator : AbstractValidator<CreateCitaDto>
{
    public CreateCitaValidator()
    {
        RuleFor(x => x.ClienteId).NotEmpty().WithMessage("El cliente es requerido.");
        RuleFor(x => x.VehiculoId).NotEmpty().WithMessage("El vehículo es requerido.");
        RuleFor(x => x.FechaHora).GreaterThan(DateTime.UtcNow).WithMessage("La fecha debe ser futura.");
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(500).WithMessage("El motivo es requerido (máx 500 caracteres).");
    }
}
