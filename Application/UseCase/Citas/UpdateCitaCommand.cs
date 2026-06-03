using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Citas;

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
        var cita = await _context.Citas
            .Include(c => c.Cliente)
            .Include(c => c.Vehiculo)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Cita", request.Id);
        _mapper.Map(request.Dto, cita);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CitaDto>(cita);
    }
}
