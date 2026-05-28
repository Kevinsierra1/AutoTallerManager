using MediatR;
using Application.Abstractions;
using Domain.Entities;

namespace Application.UseCase.Proveedores;

public record CreateProveedorCommand(CreateProveedorDto Dto) : IRequest<ProveedorDto>;

public class CreateProveedorCommandHandler : IRequestHandler<CreateProveedorCommand, ProveedorDto>
{
    private readonly IApplicationDbContext _context;
    public CreateProveedorCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<ProveedorDto> Handle(CreateProveedorCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var p = new Proveedor
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            RazonSocial = dto.RazonSocial,
            Nit = dto.Nit,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Direccion = dto.Direccion,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        _context.Proveedores.Add(p);
        await _context.SaveChangesAsync(cancellationToken);
        return new ProveedorDto(p.Id, p.Nombre, p.RazonSocial, p.Nit, p.Telefono, p.Email, p.Direccion, p.Activo, p.CreadoEn);
    }
}
