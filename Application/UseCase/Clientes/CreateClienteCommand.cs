using MediatR;
using AutoMapper;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Clientes;

public record CreateClienteCommand(CreateClienteDto Dto) : IRequest<ClienteDto>;

public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, ClienteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateClienteCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClienteDto> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = _mapper.Map<Cliente>(request.Dto);

        // Garantizar que el Id siempre sea un GUID único nuevo
        cliente.Id = Guid.NewGuid();
        cliente.CreadoEn = DateTime.UtcNow;

        // Resolver TipoDocumentoId a partir de la abreviatura (ej. "CC", "NIT")
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

        // Recargar con la navegación para que el DTO devuelva TipoDocumento
        var guardado = await _context.Clientes
            .Include(c => c.TipoDocumento)
            .FirstAsync(c => c.Id == cliente.Id, cancellationToken);

        return _mapper.Map<ClienteDto>(guardado);
    }
}
