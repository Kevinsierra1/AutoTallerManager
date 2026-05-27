using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoTallerManager.Application.Features.Facturas.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;

namespace AutoTallerManager.Application.Features.Facturas.Queries;

public record GetFacturaByIdQuery(Guid Id) : IRequest<FacturaDto>;

public class GetFacturaByIdQueryHandler : IRequestHandler<GetFacturaByIdQuery, FacturaDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetFacturaByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FacturaDto> Handle(GetFacturaByIdQuery request, CancellationToken cancellationToken)
    {
        var factura = await _context.Facturas
            .Include(f => f.Cliente)
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Factura", request.Id);
        return _mapper.Map<FacturaDto>(factura);
    }
}
