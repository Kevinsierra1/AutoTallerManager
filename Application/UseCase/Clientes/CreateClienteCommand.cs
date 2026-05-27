using MediatR;
using AutoMapper;
using Application.Abstractions;
using Domain.Entities;

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
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ClienteDto>(cliente);
    }
}
