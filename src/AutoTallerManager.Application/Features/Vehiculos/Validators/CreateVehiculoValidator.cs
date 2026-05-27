using FluentValidation;
using AutoTallerManager.Application.Features.Vehiculos.DTOs;

namespace AutoTallerManager.Application.Features.Vehiculos.Validators;

public class CreateVehiculoValidator : AbstractValidator<CreateVehiculoDto>
{
    public CreateVehiculoValidator()
    {
        RuleFor(x => x.Placa).NotEmpty().MaximumLength(20).WithMessage("La placa es requerida (máx 20 caracteres).");
        RuleFor(x => x.ModeloVehiculoId).NotEmpty().WithMessage("El modelo es requerido.");
        RuleFor(x => x.Anio).InclusiveBetween(1900, DateTime.Now.Year + 1).WithMessage("Año inválido.");
        RuleFor(x => x.Vin).MaximumLength(17).When(x => !string.IsNullOrEmpty(x.Vin));
    }
}
