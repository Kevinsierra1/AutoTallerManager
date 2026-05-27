using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Clientes;

public record DeleteClienteCommand(Guid Id) : IRequest;

public class DeleteClienteCommandHandler : IRequestHandler<DeleteClienteCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteClienteCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Id);
        cliente.Eliminado = true;
        cliente.EliminadoEn = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
