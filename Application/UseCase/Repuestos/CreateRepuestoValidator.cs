using FluentValidation;

namespace Application.UseCase.Repuestos;

public class CreateRepuestoValidator : AbstractValidator<CreateRepuestoDto>
{
    public CreateRepuestoValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CategoriaRepuestoId).NotEmpty();
        RuleFor(x => x.PrecioCompra).GreaterThan(0);
        RuleFor(x => x.PrecioVenta).GreaterThan(0);
        RuleFor(x => x.StockMinimo).GreaterThanOrEqualTo(0);
    }
}
