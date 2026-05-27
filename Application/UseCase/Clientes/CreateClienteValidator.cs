using FluentValidation;

namespace Application.UseCase.Clientes;

public class CreateClienteValidator : AbstractValidator<CreateClienteDto>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100).WithMessage("Nombres requeridos (máx 100).");
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(100).WithMessage("Apellidos requeridos (máx 100).");
        RuleFor(x => x.TipoDocumento).NotEmpty().WithMessage("Tipo de documento requerido.");
        RuleFor(x => x.NumeroDocumento).NotEmpty().MaximumLength(20).WithMessage("Número de documento requerido.");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email inválido.");
    }
}
