using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Clientes;

public record GetClienteByIdQuery(Guid Id) : IRequest<ClienteDto>;

public class GetClienteByIdQueryHandler : IRequestHandler<GetClienteByIdQuery, ClienteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClienteByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClienteDto> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes
            .Include(c => c.TipoDocumento)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Id);
        return _mapper.Map<ClienteDto>(cliente);
    }
}
