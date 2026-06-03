using MediatR;
using Application.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Auth;

public record RegisterClienteCommand(RegisterClienteDto Dto) : IRequest<AuthResponseDto>;

public class RegisterClienteCommandHandler : IRequestHandler<RegisterClienteCommand, AuthResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public RegisterClienteCommandHandler(IApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(RegisterClienteCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email, cancellationToken))
            throw new DomainException("El email ya está registrado.");

        if (await _context.Vehiculos.AnyAsync(v => v.Placa == dto.Placa.ToUpperInvariant(), cancellationToken))
            throw new DomainException($"La placa {dto.Placa.ToUpper()} ya está registrada en el sistema.");

        // 1. Usuario con rol Cliente
        var usuario = new Usuario
        {
            Id           = Guid.NewGuid(),
            Email        = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Nombres      = dto.Nombres,
            Apellidos    = dto.Apellidos,
            Activo       = true,
            CreadoEn     = DateTime.UtcNow
        };

        var rolCliente = await _context.Roles
            .FirstOrDefaultAsync(r => r.Nombre == "Cliente", cancellationToken);
        if (rolCliente != null)
            usuario.UsuarioRoles = new List<UsuarioRol>
            {
                new() { UsuarioId = usuario.Id, RolId = rolCliente.Id }
            };

        _context.Usuarios.Add(usuario);

        // 2. Perfil Cliente
        var tipoDoc = await _context.TiposDocumento
            .FirstOrDefaultAsync(t =>
                t.Abreviatura == dto.TipoDocumento || t.Nombre == dto.TipoDocumento,
                cancellationToken);

        var cliente = new Cliente
        {
            Id              = Guid.NewGuid(),
            Nombres         = dto.Nombres,
            Apellidos       = dto.Apellidos,
            TipoDocumentoId = tipoDoc?.Id,
            NumeroDocumento = dto.NumeroDocumento,
            Email           = dto.Email,
            Telefono        = dto.Telefono,
            Activo          = true,
            UsuarioId       = usuario.Id,
            CreadoEn        = DateTime.UtcNow
        };
        _context.Clientes.Add(cliente);

        // 3. Vehículo
        var vehiculo = new Vehiculo
        {
            Id                = Guid.NewGuid(),
            Placa             = dto.Placa.ToUpperInvariant(),
            Vin               = dto.Vin,
            ModeloVehiculoId  = dto.ModeloVehiculoId,
            ColorId           = dto.ColorId,
            Anio              = dto.Anio,
            KilometrajeActual = 0,
            Activo            = true,
            CreadoEn          = DateTime.UtcNow
        };
        _context.Vehiculos.Add(vehiculo);

        // 4. Relación vehículo-cliente
        _context.VehiculoPropietarios.Add(new VehiculoPropietario
        {
            Id          = Guid.NewGuid(),
            VehiculoId  = vehiculo.Id,
            ClienteId   = cliente.Id,
            FechaInicio = DateTime.UtcNow,
            Activo      = true,
            CreadoEn    = DateTime.UtcNow
        });

        // 5. Token JWT — generado antes del save para incluirlo en el mismo commit
        var roles        = new[] { "Cliente" };
        var token        = _jwtService.GenerarToken(usuario, roles);
        var refreshToken = _jwtService.GenerarRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token      = refreshToken,
            UsuarioId  = usuario.Id,
            Expiracion = DateTime.UtcNow.AddDays(7),
            Revocado   = false
        });

        // Un único SaveChanges — todo atómico
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(token, refreshToken, DateTime.UtcNow.AddHours(1),
            roles, usuario.Id, usuario.Email, usuario.Nombres, usuario.Apellidos);
    }
}
