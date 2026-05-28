using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Proveedores;

public record UpdateProveedorCommand(Guid Id, UpdateProveedorDto Dto) : IRequest<ProveedorDto>;

public class UpdateProveedorCommandHandler : IRequestHandler<UpdateProveedorCommand, ProveedorDto>
{
    private readonly IApplicationDbContext _context;
    public UpdateProveedorCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<ProveedorDto> Handle(UpdateProveedorCommand request, CancellationToken cancellationToken)
    {
        var p = await _context.Proveedores
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.Eliminado, cancellationToken)
            ?? throw new NotFoundException("Proveedor", request.Id);

        var dto = request.Dto;
        if (dto.Nombre != null) p.Nombre = dto.Nombre;
        if (dto.RazonSocial != null) p.RazonSocial = dto.RazonSocial;
        if (dto.Nit != null) p.Nit = dto.Nit;
        if (dto.Telefono != null) p.Telefono = dto.Telefono;
        if (dto.Email != null) p.Email = dto.Email;
        if (dto.Direccion != null) p.Direccion = dto.Direccion;
        if (dto.Activo.HasValue) p.Activo = dto.Activo.Value;
        p.ActualizadoEn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return new ProveedorDto(p.Id, p.Nombre, p.RazonSocial, p.Nit, p.Telefono, p.Email, p.Direccion, p.Activo, p.CreadoEn);
    }
}
