using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Clientes;

public record UpdateClienteCommand(Guid Id, UpdateClienteDto Dto) : IRequest<ClienteDto>;

public class UpdateClienteCommandHandler : IRequestHandler<UpdateClienteCommand, ClienteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateClienteCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClienteDto> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes
            .Include(c => c.TipoDocumento)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Id);
        _mapper.Map(request.Dto, cliente);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ClienteDto>(cliente);
    }
}
