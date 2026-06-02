using FluentValidation;

namespace Application.UseCase.Empleados;

public class CreateEmpleadoValidator : AbstractValidator<CreateEmpleadoCommand>
{
    public CreateEmpleadoValidator()
    {
        RuleFor(x => x.Dto.Nombres).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dto.Apellidos).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Dto.NumeroDocumento).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Dto.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Dto.Email));
    }
}
