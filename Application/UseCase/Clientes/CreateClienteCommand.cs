using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.UseCase.Clientes;

public record CreateClienteCommand(CreateClienteDto Dto) : IRequest<ClienteCreadoDto>;

public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, ClienteCreadoDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateClienteCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ClienteCreadoDto> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        if (await _uow.Repository<Usuario>().ExisteAsync(u => u.Email == request.Dto.Email, cancellationToken))
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

        var roles = await _uow.Repository<Rol>().BuscarAsync(r => r.Nombre == "Cliente", cancellationToken);
        var rolCliente = roles.FirstOrDefault();
        if (rolCliente != null)
        {
            usuario.UsuarioRoles = new List<UsuarioRol>
            {
                new UsuarioRol { UsuarioId = usuario.Id, RolId = rolCliente.Id }
            };
        }

        await _uow.Repository<Usuario>().AgregarAsync(usuario, cancellationToken);

        var cliente = _mapper.Map<Cliente>(request.Dto);
        cliente.Id = Guid.NewGuid();
        cliente.CreadoEn = DateTime.UtcNow;
        cliente.UsuarioId = usuario.Id;

        if (!string.IsNullOrEmpty(request.Dto.TipoDocumento))
        {
            var tipos = await _uow.Repository<TipoDocumento>().BuscarAsync(
                t => t.Abreviatura == request.Dto.TipoDocumento || t.Nombre == request.Dto.TipoDocumento,
                cancellationToken);
            cliente.TipoDocumentoId = tipos.FirstOrDefault()?.Id;
        }

        await _uow.Repository<Cliente>().AgregarAsync(cliente, cancellationToken);

        // Usuario + Cliente se persisten en una sola transacción atómica
        await _uow.IniciarTransaccionAsync(cancellationToken);
        try
        {
            await _uow.GuardarCambiosAsync(cancellationToken);
            await _uow.ConfirmarTransaccionAsync(cancellationToken);
        }
        catch
        {
            await _uow.RevertirTransaccionAsync(cancellationToken);
            throw;
        }

        var guardado = await _uow.Repository<Cliente>().Query()
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
