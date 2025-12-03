namespace TestEv.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username, string role);
        bool ValidateToken(string token);
        int GetExpirationSeconds();
    }
}
