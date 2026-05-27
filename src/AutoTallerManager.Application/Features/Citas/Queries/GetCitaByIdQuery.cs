using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoTallerManager.Application.Features.Citas.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;

namespace AutoTallerManager.Application.Features.Citas.Queries;

public record GetCitaByIdQuery(Guid Id) : IRequest<CitaDto>;

public class GetCitaByIdQueryHandler : IRequestHandler<GetCitaByIdQuery, CitaDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCitaByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CitaDto> Handle(GetCitaByIdQuery request, CancellationToken cancellationToken)
    {
        var cita = await _context.Citas
            .Include(c => c.Cliente).Include(c => c.Vehiculo)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Cita", request.Id);
        return _mapper.Map<CitaDto>(cita);
    }
}
