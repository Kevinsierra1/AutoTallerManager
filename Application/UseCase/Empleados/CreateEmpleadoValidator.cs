using FluentValidation;

namespace Application.UseCase.Empleados;

public class CreateEmpleadoValidator : AbstractValidator<CreateEmpleadoDto>
{
    public CreateEmpleadoValidator()
    {
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NumeroDocumento).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}
