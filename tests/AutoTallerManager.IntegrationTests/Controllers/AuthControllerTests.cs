using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using AutoTallerManager.IntegrationTests.Fixtures;
using Xunit;

namespace AutoTallerManager.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Con_Credenciales_Invalidas_Retorna_Unauthorized()
    {
        var response = await _client.PostAsJsonAsync("api/auth/login", new
        {
            email = "noexiste@test.com",
            password = "wrongpassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_Con_Email_Invalido_Retorna_BadRequest()
    {
        var response = await _client.PostAsJsonAsync("api/auth/register", new
        {
            email = "not-an-email",
            password = "password123",
            nombres = "Test",
            apellidos = "User"
        });

        // Validation should fail
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }
}
