using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoTallerManager.Application.Features.Repuestos.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;

namespace AutoTallerManager.Application.Features.Repuestos.Queries;

public record GetRepuestoByIdQuery(Guid Id) : IRequest<RepuestoDto>;

public class GetRepuestoByIdQueryHandler : IRequestHandler<GetRepuestoByIdQuery, RepuestoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRepuestoByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<RepuestoDto> Handle(GetRepuestoByIdQuery request, CancellationToken cancellationToken)
    {
        var repuesto = await _context.Repuestos
            .Include(r => r.CategoriaRepuesto)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Repuesto", request.Id);
        return _mapper.Map<RepuestoDto>(repuesto);
    }
}
