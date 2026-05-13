using Moq;
using NUnit.Framework;
using FlowBoard.AuthService.Services;
using FlowBoard.AuthService.Interfaces;
using FlowBoard.AuthService.Entities;
using FlowBoard.AuthService.DTOs;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace FlowBoard.UnitTests;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _repoMock;
    private Mock<IConfiguration> _configMock;
    private Mock<IMapper> _mapperMock;
    private AuthServiceImpl _authService;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IUserRepository>();
        _configMock = new Mock<IConfiguration>();
        _mapperMock = new Mock<IMapper>();

        _configMock.Setup(c => c["Jwt:Key"]).Returns("super-secret-key-that-is-long-enough-32-chars");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("FlowBoard");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("FlowBoardUsers");

        _authService = new AuthServiceImpl(_repoMock.Object, _configMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task LoginAsync_ShouldReturnToken_WhenValid()
    {
        // Arrange
        var password = "password123";
        var user = new User
        {
            UserId = 1,
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = true
        };

        _repoMock.Setup(r => r.FindByEmailAsync(user.Email)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(new LoginDto { Email = user.Email, Password = password });

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
    }
}
