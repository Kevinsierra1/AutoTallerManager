using FluentValidation;

namespace Application.UseCase.Vehiculos;

public class CreateVehiculoValidator : AbstractValidator<CreateVehiculoCommand>
{
    public CreateVehiculoValidator()
    {
        RuleFor(x => x.Dto.Placa).NotEmpty().MaximumLength(20).WithMessage("La placa es requerida (máx 20 caracteres).");
        RuleFor(x => x.Dto.ModeloVehiculoId).NotEmpty().WithMessage("El modelo es requerido.");
        RuleFor(x => x.Dto.Anio).InclusiveBetween(1900, DateTime.Now.Year + 1).WithMessage("Año inválido.");
        RuleFor(x => x.Dto.Vin).MaximumLength(17).When(x => !string.IsNullOrEmpty(x.Dto.Vin));
    }
}
