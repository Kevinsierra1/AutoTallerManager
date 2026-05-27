using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Citas;

public record DeleteCitaCommand(Guid Id) : IRequest;

public class DeleteCitaCommandHandler : IRequestHandler<DeleteCitaCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCitaCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteCitaCommand request, CancellationToken cancellationToken)
    {
        var cita = await _context.Citas.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Cita", request.Id);
        cita.Eliminado = true;
        cita.EliminadoEn = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
