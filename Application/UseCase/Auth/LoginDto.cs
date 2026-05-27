namespace Application.UseCase.Auth;

public record LoginDto(string Email, string Password);
public record RegisterDto(string Email, string Password, string Nombres, string Apellidos);
public record RefreshTokenDto(string Token, string RefreshToken);
public record RevokeTokenDto(string RefreshToken);
public record AuthResponseDto(
    string Token,
    string RefreshToken,
    DateTime Expiration,
    string[] Roles,
    Guid UserId,
    string Email,
    string Nombres,
    string Apellidos
);
