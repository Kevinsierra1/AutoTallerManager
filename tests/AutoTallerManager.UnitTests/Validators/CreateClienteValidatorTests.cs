using FluentAssertions;
using AutoTallerManager.Application.Features.Clientes.DTOs;
using AutoTallerManager.Application.Features.Clientes.Validators;
using Xunit;

namespace AutoTallerManager.UnitTests.Validators;

public class CreateClienteValidatorTests
{
    private readonly CreateClienteValidator _validator = new();

    [Fact]
    public async Task Debe_Fallar_Cuando_NombresEsVacio()
    {
        var dto = new CreateClienteDto("", "Pérez", "CC", "123456", null, null);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nombres");
    }

    [Fact]
    public async Task Debe_Fallar_Cuando_NumeroDocumentoEsVacio()
    {
        var dto = new CreateClienteDto("Juan", "Pérez", "CC", "", null, null);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Debe_Ser_Valido_Con_Datos_Correctos()
    {
        var dto = new CreateClienteDto("Juan", "Pérez", "CC", "123456789", null, null);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Debe_Fallar_Con_Email_Invalido()
    {
        var dto = new CreateClienteDto("Juan", "Pérez", "CC", "123456", "no-es-email", null);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}
