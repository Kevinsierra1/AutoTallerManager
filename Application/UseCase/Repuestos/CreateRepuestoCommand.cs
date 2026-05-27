using MediatR;
using AutoMapper;
using Application.Abstractions;
using Domain.Entities;

namespace Application.UseCase.Repuestos;

public record CreateRepuestoCommand(CreateRepuestoDto Dto) : IRequest<RepuestoDto>;

public class CreateRepuestoCommandHandler : IRequestHandler<CreateRepuestoCommand, RepuestoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateRepuestoCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<RepuestoDto> Handle(CreateRepuestoCommand request, CancellationToken cancellationToken)
    {
        var repuesto = _mapper.Map<Repuesto>(request.Dto);
        repuesto.Id = Guid.NewGuid();
        repuesto.CreadoEn = DateTime.UtcNow;
        repuesto.Activo = true;
        _context.Repuestos.Add(repuesto);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<RepuestoDto>(repuesto);
    }
}
