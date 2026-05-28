using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Empleados;

public record DeleteEmpleadoCommand(Guid Id) : IRequest;

public class DeleteEmpleadoCommandHandler : IRequestHandler<DeleteEmpleadoCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteEmpleadoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteEmpleadoCommand request, CancellationToken cancellationToken)
    {
        var empleado = await _context.Empleados.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Empleado", request.Id);
        empleado.Eliminado = true;
        empleado.EliminadoEn = DateTime.UtcNow;
        empleado.Activo = false;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
