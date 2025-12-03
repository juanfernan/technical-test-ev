namespace TestEv.Application.DTOs
{
    public record LoginRequest(
        string Username,
        string Password);

    public record AuthResponse(
        string Token,
        string TokenType,
        int ExpiresIn,
        string Username,
        string Role);
}
