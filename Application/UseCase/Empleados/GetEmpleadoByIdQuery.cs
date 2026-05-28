using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using AutoMapper;

namespace Application.UseCase.Empleados;

public record GetEmpleadoByIdQuery(Guid Id) : IRequest<EmpleadoDto>;

public class GetEmpleadoByIdQueryHandler : IRequestHandler<GetEmpleadoByIdQuery, EmpleadoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetEmpleadoByIdQueryHandler(IApplicationDbContext context, IMapper mapper) { _context = context; _mapper = mapper; }

    public async Task<EmpleadoDto> Handle(GetEmpleadoByIdQuery request, CancellationToken cancellationToken)
    {
        var empleado = await _context.Empleados.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Empleado", request.Id);
        if (empleado.Eliminado) throw new NotFoundException("Empleado", request.Id);
        return _mapper.Map<EmpleadoDto>(empleado);
    }
}
