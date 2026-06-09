using MediatR;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.UseCase.Clientes;

public record DeleteClienteCommand(Guid Id) : IRequest;

public class DeleteClienteCommandHandler : IRequestHandler<DeleteClienteCommand>
{
    private readonly IUnitOfWork _uow;
    public DeleteClienteCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _uow.Repository<Cliente>().ObtenerPorIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Id);
        _uow.Repository<Cliente>().Eliminar(cliente);
        await _uow.GuardarCambiosAsync(cancellationToken);
    }
}
