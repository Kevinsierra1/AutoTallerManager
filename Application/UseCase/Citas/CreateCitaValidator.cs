using FluentValidation;

namespace Application.UseCase.Citas;

public class CreateCitaValidator : AbstractValidator<CreateCitaCommand>
{
    public CreateCitaValidator()
    {
        RuleFor(x => x.Dto.ClienteId).NotEmpty().WithMessage("El cliente es requerido.");
        RuleFor(x => x.Dto.VehiculoId).NotEmpty().WithMessage("El vehículo es requerido.");
        RuleFor(x => x.Dto.FechaHora).GreaterThan(DateTime.UtcNow).WithMessage("La fecha debe ser futura.");
        RuleFor(x => x.Dto.Motivo).NotEmpty().MaximumLength(500).WithMessage("El motivo es requerido (máx 500 caracteres).");
    }
}
