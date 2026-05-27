using MediatR;
using AutoMapper;
using AutoTallerManager.Application.Features.Repuestos.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;

namespace AutoTallerManager.Application.Features.Repuestos.Commands;

public record UpdateRepuestoCommand(Guid Id, UpdateRepuestoDto Dto) : IRequest<RepuestoDto>;

public class UpdateRepuestoCommandHandler : IRequestHandler<UpdateRepuestoCommand, RepuestoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateRepuestoCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<RepuestoDto> Handle(UpdateRepuestoCommand request, CancellationToken cancellationToken)
    {
        var repuesto = await _context.Repuestos.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Repuesto", request.Id);
        _mapper.Map(request.Dto, repuesto);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<RepuestoDto>(repuesto);
    }
}
