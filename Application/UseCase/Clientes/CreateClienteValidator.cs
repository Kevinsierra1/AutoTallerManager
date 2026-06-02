using FluentValidation;

namespace Application.UseCase.Clientes;

public class CreateClienteValidator : AbstractValidator<CreateClienteCommand>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.Dto.Nombres).NotEmpty().MaximumLength(100).WithMessage("Nombres requeridos (máx 100).");
        RuleFor(x => x.Dto.Apellidos).NotEmpty().MaximumLength(100).WithMessage("Apellidos requeridos (máx 100).");
        RuleFor(x => x.Dto.TipoDocumento).NotEmpty().WithMessage("Tipo de documento requerido.");
        RuleFor(x => x.Dto.NumeroDocumento).NotEmpty().MaximumLength(20).WithMessage("Número de documento requerido.");
        RuleFor(x => x.Dto.Email).NotEmpty().WithMessage("El email es obligatorio.");
        RuleFor(x => x.Dto.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Dto.Email)).WithMessage("El email no tiene un formato válido.");
    }
}
