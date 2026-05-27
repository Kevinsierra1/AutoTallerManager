using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoTallerManager.Application.Features.Citas.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;

namespace AutoTallerManager.Application.Features.Citas.Commands;

public record UpdateCitaCommand(Guid Id, UpdateCitaDto Dto) : IRequest<CitaDto>;

public class UpdateCitaCommandHandler : IRequestHandler<UpdateCitaCommand, CitaDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateCitaCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CitaDto> Handle(UpdateCitaCommand request, CancellationToken cancellationToken)
    {
        var cita = await _context.Citas.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Cita", request.Id);
        _mapper.Map(request.Dto, cita);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CitaDto>(cita);
    }
}
