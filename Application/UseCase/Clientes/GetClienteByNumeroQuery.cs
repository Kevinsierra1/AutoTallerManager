using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Clientes;

public record GetClienteByNumeroQuery(int Numero) : IRequest<ClienteDto>;

public class GetClienteByNumeroQueryHandler : IRequestHandler<GetClienteByNumeroQuery, ClienteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClienteByNumeroQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClienteDto> Handle(GetClienteByNumeroQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes
            .Include(c => c.TipoDocumento)
            .FirstOrDefaultAsync(c => c.Numero == request.Numero, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Numero);
        return _mapper.Map<ClienteDto>(cliente);
    }
}
