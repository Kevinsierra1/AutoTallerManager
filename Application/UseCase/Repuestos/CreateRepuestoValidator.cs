using FluentValidation;

namespace Application.UseCase.Repuestos;

public class CreateRepuestoValidator : AbstractValidator<CreateRepuestoCommand>
{
    public CreateRepuestoValidator()
    {
        RuleFor(x => x.Dto.Codigo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Dto.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.CategoriaRepuestoId).NotEmpty();
        RuleFor(x => x.Dto.PrecioCompra).GreaterThan(0);
        RuleFor(x => x.Dto.PrecioVenta).GreaterThan(0);
        RuleFor(x => x.Dto.StockMinimo).GreaterThanOrEqualTo(0);
    }
}
