using FluentAssertions;
using AutoMapper;
using Moq;
using AutoTallerManager.Application.Features.Clientes.Commands;
using AutoTallerManager.Application.Features.Clientes.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoTallerManager.UnitTests.UseCases;

public class CreateClienteCommandTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreateClienteCommandHandler _handler;

    public CreateClienteCommandTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockMapper = new Mock<IMapper>();
        _handler = new CreateClienteCommandHandler(_mockContext.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_Debe_Crear_Cliente_Y_Retornar_Dto()
    {
        // Arrange
        var dto = new CreateClienteDto("Juan", "Pérez", "CC", "123456789", null, null);
        var cliente = new Cliente { Id = Guid.NewGuid(), Nombres = "Juan", Apellidos = "Pérez" };
        var clienteDto = new ClienteDto(cliente.Id, "Juan", "Pérez", "CC", "123456789", null, null, null, DateTime.UtcNow);

        _mockMapper.Setup(m => m.Map<Cliente>(dto)).Returns(cliente);
        _mockMapper.Setup(m => m.Map<ClienteDto>(cliente)).Returns(clienteDto);

        var mockSet = new Mock<DbSet<Cliente>>();
        _mockContext.Setup(c => c.Clientes).Returns(mockSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(new CreateClienteCommand(dto), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Nombres.Should().Be("Juan");
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
