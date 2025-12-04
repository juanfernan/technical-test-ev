using TestEv.Application.Interfaces;
using TestEv.Domain.Exceptions;
using FluentAssertions;
using Moq;
using TestEv.Application.Services;
using TestEv.Application.DTOs;

namespace TestEv.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _sut = new AuthService(_jwtTokenServiceMock.Object, "admin", "Admin123!");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
        {
            var request = new LoginRequest("admin", "Admin123!");
            _jwtTokenServiceMock.Setup(x => x.GenerateToken("admin", "Admin")).Returns("jwt-token");
            _jwtTokenServiceMock.Setup(x => x.GetExpirationSeconds()).Returns(3600);

            var result = await _sut.LoginAsync(request);

            result.Should().NotBeNull();
            result.Token.Should().Be("jwt-token");
            result.TokenType.Should().Be("Bearer");
            result.ExpiresIn.Should().Be(3600);
            result.Username.Should().Be("admin");
            result.Role.Should().Be("Admin");
        }

        [Fact]
        public async Task LoginAsync_WithInvalidUsername_ThrowsAuthenticationException()
        {
            var request = new LoginRequest("wronguser", "Admin123!");

            var act = () => _sut.LoginAsync(request);

            await act.Should().ThrowAsync<AuthenticationException>()
                .WithMessage("Invalid username or password");
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ThrowsAuthenticationException()
        {
            var request = new LoginRequest("admin", "wrongpassword");

            var act = () => _sut.LoginAsync(request);

            await act.Should().ThrowAsync<AuthenticationException>()
                .WithMessage("Invalid username or password");
        }
    }
}
