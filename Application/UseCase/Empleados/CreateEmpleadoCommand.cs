using MediatR;
using AutoMapper;
using Application.Abstractions;
using Domain.Entities;

namespace Application.UseCase.Empleados;

public record CreateEmpleadoCommand(CreateEmpleadoDto Dto) : IRequest<EmpleadoDto>;

public class CreateEmpleadoCommandHandler : IRequestHandler<CreateEmpleadoCommand, EmpleadoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateEmpleadoCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EmpleadoDto> Handle(CreateEmpleadoCommand request, CancellationToken cancellationToken)
    {
        var empleado = _mapper.Map<Empleado>(request.Dto);
        empleado.Id = Guid.NewGuid();
        empleado.CreadoEn = DateTime.UtcNow;
        empleado.Activo = true;
        _context.Empleados.Add(empleado);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<EmpleadoDto>(empleado);
    }
}
