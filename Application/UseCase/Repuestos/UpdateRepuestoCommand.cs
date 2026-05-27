using MediatR;
using AutoMapper;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Repuestos;

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
