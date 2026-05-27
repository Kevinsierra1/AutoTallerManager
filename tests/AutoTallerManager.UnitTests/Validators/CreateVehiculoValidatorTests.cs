using FluentAssertions;
using AutoTallerManager.Application.Features.Vehiculos.DTOs;
using AutoTallerManager.Application.Features.Vehiculos.Validators;
using Xunit;

namespace AutoTallerManager.UnitTests.Validators;

public class CreateVehiculoValidatorTests
{
    private readonly CreateVehiculoValidator _validator = new();

    [Fact]
    public async Task Debe_Fallar_Cuando_PlacaEsVacia()
    {
        var dto = new CreateVehiculoDto("", null, Guid.NewGuid(), Guid.NewGuid(), 2020, null);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Placa");
    }

    [Fact]
    public async Task Debe_Fallar_Cuando_AnioInvalido()
    {
        var dto = new CreateVehiculoDto("ABC123", null, Guid.NewGuid(), Guid.NewGuid(), 1800, null);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Debe_Ser_Valido_Con_Datos_Correctos()
    {
        var dto = new CreateVehiculoDto("ABC123", "VIN12345678901234", Guid.NewGuid(), Guid.NewGuid(), 2022, null);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }
}
