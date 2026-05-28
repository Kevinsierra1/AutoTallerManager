using MediatR;
using Application.Abstractions;
using Domain.Entities;

namespace Application.UseCase.Roles;

public record CreateRolCommand(CreateRolDto Dto) : IRequest<RolDto>;

public class CreateRolCommandHandler : IRequestHandler<CreateRolCommand, RolDto>
{
    private readonly IApplicationDbContext _context;
    public CreateRolCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<RolDto> Handle(CreateRolCommand request, CancellationToken cancellationToken)
    {
        var rol = new Rol
        {
            Id = Guid.NewGuid(),
            Nombre = request.Dto.Nombre,
            Descripcion = request.Dto.Descripcion,
            CreadoEn = DateTime.UtcNow
        };
        _context.Roles.Add(rol);
        await _context.SaveChangesAsync(cancellationToken);
        return new RolDto(rol.Id, rol.Nombre, rol.Descripcion, [], rol.CreadoEn);
    }
}
