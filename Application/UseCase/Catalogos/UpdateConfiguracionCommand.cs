using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Catalogos;

public record UpdateConfiguracionCommand(string Clave, string Valor) : IRequest<ConfiguracionItemDto>;

public class UpdateConfiguracionCommandHandler : IRequestHandler<UpdateConfiguracionCommand, ConfiguracionItemDto>
{
    private readonly IApplicationDbContext _context;
    public UpdateConfiguracionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<ConfiguracionItemDto> Handle(UpdateConfiguracionCommand request, CancellationToken ct)
    {
        var config = await _context.ConfiguracionesSistema
            .FirstOrDefaultAsync(c => c.Clave == request.Clave && c.Activo, ct)
            ?? throw new NotFoundException("ConfiguracionSistema", request.Clave);

        config.Valor = request.Valor;
        await _context.SaveChangesAsync(ct);

        return new ConfiguracionItemDto(config.Id, config.Clave, config.Valor,
            config.Tipo, config.Grupo, config.Descripcion, config.Activo);
    }
}
