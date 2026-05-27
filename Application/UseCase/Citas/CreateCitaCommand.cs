using MediatR;
using AutoMapper;
using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.Citas;

public record CreateCitaCommand(CreateCitaDto Dto) : IRequest<CitaDto>;

public class CreateCitaCommandHandler : IRequestHandler<CreateCitaCommand, CitaDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateCitaCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CitaDto> Handle(CreateCitaCommand request, CancellationToken cancellationToken)
    {
        var cita = _mapper.Map<Cita>(request.Dto);
        cita.Estado = EstadoCitaEnum.Pendiente;
        _context.Citas.Add(cita);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CitaDto>(cita);
    }
}
