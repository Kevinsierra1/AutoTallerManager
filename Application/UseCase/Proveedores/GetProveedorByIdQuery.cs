using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Proveedores;

public record GetProveedorByIdQuery(Guid Id) : IRequest<ProveedorDto>;

public class GetProveedorByIdQueryHandler : IRequestHandler<GetProveedorByIdQuery, ProveedorDto>
{
    private readonly IApplicationDbContext _context;
    public GetProveedorByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<ProveedorDto> Handle(GetProveedorByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _context.Proveedores
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.Eliminado, cancellationToken)
            ?? throw new NotFoundException("Proveedor", request.Id);
        return new ProveedorDto(p.Id, p.Nombre, p.RazonSocial, p.Nit, p.Telefono, p.Email, p.Direccion, p.Activo, p.CreadoEn);
    }
}
