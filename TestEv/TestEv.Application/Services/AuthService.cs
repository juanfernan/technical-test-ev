using TestEv.Application.DTOs;
using TestEv.Application.Interfaces;
using TestEv.Domain.Exceptions;

namespace TestEv.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly string _adminUsername;
        private readonly string _adminPassword;

        public AuthService(IJwtTokenService jwtTokenService, string adminUsername, string adminPassword)
        {
            _jwtTokenService = jwtTokenService;
            _adminUsername = adminUsername;
            _adminPassword = adminPassword;
        }

        public Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Username != _adminUsername || request.Password != _adminPassword)
            {
                throw new AuthenticationException("Invalid username or password");
            }

            var token = _jwtTokenService.GenerateToken(_adminUsername, "Admin");

            return Task.FromResult(new AuthResponse(
                Token: token,
                TokenType: "Bearer",
                ExpiresIn: _jwtTokenService.GetExpirationSeconds(),
                Username: _adminUsername,
                Role: "Admin"));
        }
    }
}
