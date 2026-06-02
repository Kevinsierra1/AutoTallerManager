using MediatR;
using AutoMapper;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Clientes;

public record CreateClienteCommand(CreateClienteDto Dto) : IRequest<ClienteCreadoDto>;

public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, ClienteCreadoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateClienteCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClienteCreadoDto> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == request.Dto.Email, cancellationToken))
            throw new Domain.Exceptions.DomainException("El email ya está registrado en el sistema.");

        var contrasena = GenerarContrasenaTemporal();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(contrasena);

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = request.Dto.Email,
            PasswordHash = passwordHash,
            Nombres = request.Dto.Nombres,
            Apellidos = request.Dto.Apellidos,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };

        var rolCliente = await _context.Roles
            .FirstOrDefaultAsync(r => r.Nombre == "Cliente", cancellationToken);
        if (rolCliente != null)
        {
            usuario.UsuarioRoles = new List<UsuarioRol>
            {
                new UsuarioRol { UsuarioId = usuario.Id, RolId = rolCliente.Id }
            };
        }

        _context.Usuarios.Add(usuario);

        var cliente = _mapper.Map<Cliente>(request.Dto);
        cliente.Id = Guid.NewGuid();
        cliente.CreadoEn = DateTime.UtcNow;
        cliente.UsuarioId = usuario.Id;

        if (!string.IsNullOrEmpty(request.Dto.TipoDocumento))
        {
            var tipoDoc = await _context.TiposDocumento
                .FirstOrDefaultAsync(t =>
                    t.Abreviatura == request.Dto.TipoDocumento ||
                    t.Nombre == request.Dto.TipoDocumento,
                    cancellationToken);
            cliente.TipoDocumentoId = tipoDoc?.Id;
        }

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync(cancellationToken);

        var guardado = await _context.Clientes
            .Include(c => c.TipoDocumento)
            .FirstAsync(c => c.Id == cliente.Id, cancellationToken);

        return new ClienteCreadoDto(
            guardado.Id,
            guardado.Numero,
            guardado.Nombres,
            guardado.Apellidos,
            guardado.TipoDocumento?.Nombre ?? string.Empty,
            guardado.NumeroDocumento,
            guardado.Email,
            guardado.Telefono,
            guardado.Direccion,
            guardado.CreadoEn,
            guardado.UsuarioId,
            contrasena
        );
    }

    private static string GenerarContrasenaTemporal()
    {
        const string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        return new string(Enumerable.Range(0, 10)
            .Select(_ => chars[Random.Shared.Next(chars.Length)])
            .ToArray());
    }
}
