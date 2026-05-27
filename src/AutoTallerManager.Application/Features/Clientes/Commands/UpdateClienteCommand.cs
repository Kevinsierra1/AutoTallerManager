using MediatR;
using AutoMapper;
using AutoTallerManager.Application.Features.Clientes.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;

namespace AutoTallerManager.Application.Features.Clientes.Commands;

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
        var cliente = await _context.Clientes.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Id);
        _mapper.Map(request.Dto, cliente);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ClienteDto>(cliente);
    }
}
