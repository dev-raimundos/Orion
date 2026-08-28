using Orion.SharedKernel;
using Orion.SharedKernel.Contracts;
using Authentication.Application.Abstractions;
using Authentication.Application.UseCases.Login;
using Authentication.Domain;
using Authentication.Domain.Abstractions;
using Moq;

namespace Authentication.Tests.Application;

public class LoginUseCaseTests
{
    private readonly Mock<IUserCredentialsChecker> _credentialsChecker = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();
    private readonly Mock<IRefreshTokenGenerator> _refreshTokenGenerator = new();
    private readonly Mock<ILoginAttemptRepository> _loginAttempts = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokens = new();

    private LoginUseCase CreateSut() => new(
        _credentialsChecker.Object,
        _tokenGenerator.Object,
        _refreshTokenGenerator.Object,
        _loginAttempts.Object,
        _refreshTokens.Object);

    [Fact]
    public async Task ExecuteAsync_WhenLockedOut_ThrowsLockedAndNeverChecksCredentials()
    {
        var recentFailures = Enumerable.Range(0, LoginLockoutPolicy.MaxFailedAttempts)
            .Select(_ => new LoginAttempt("fulano@teste.com", succeeded: false))
            .ToList();
        _loginAttempts.Setup(r => r.GetRecentAsync("fulano@teste.com", It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recentFailures);

        var sut = CreateSut();
        var request = new LoginInput("fulano@teste.com", "qualquer-senha");

        await Assert.ThrowsAsync<AppLockedException>(() => sut.ExecuteAsync(request, CancellationToken.None));

        _credentialsChecker.Verify(c => c.ValidateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCredentialsAreInvalid_RecordsFailedAttemptAndThrowsUnauthorized()
    {
        _loginAttempts.Setup(r => r.GetRecentAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _credentialsChecker.Setup(c => c.ValidateAsync("fulano@teste.com", "senha-errada", It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthenticatedUser?)null);

        var sut = CreateSut();
        var request = new LoginInput("fulano@teste.com", "senha-errada");

        await Assert.ThrowsAsync<AppUnauthorizedException>(() => sut.ExecuteAsync(request, CancellationToken.None));

        _loginAttempts.Verify(r => r.AddAsync(
            It.Is<LoginAttempt>(a => a.Email == "fulano@teste.com" && !a.Succeeded),
            It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokens.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCredentialsAreValid_RecordsSuccessAndReturnsAccessAndRefreshTokens()
    {
        var userId = Guid.NewGuid();
        var authenticatedUser = new AuthenticatedUser(userId, "fulano@teste.com");
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(15);

        _loginAttempts.Setup(r => r.GetRecentAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _credentialsChecker.Setup(c => c.ValidateAsync("fulano@teste.com", "senha-certa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(authenticatedUser);
        _tokenGenerator.Setup(t => t.Generate(userId, "fulano@teste.com"))
            .Returns(("access-token", expiresAt));
        _refreshTokenGenerator.Setup(g => g.Generate())
            .Returns(("refresh-token", "refresh-token-hash"));

        var sut = CreateSut();
        var request = new LoginInput("fulano@teste.com", "senha-certa");

        var result = await sut.ExecuteAsync(request, CancellationToken.None);

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal(expiresAt, result.ExpiresAt);
        Assert.Equal("refresh-token", result.RefreshToken);

        _loginAttempts.Verify(r => r.AddAsync(
            It.Is<LoginAttempt>(a => a.Email == "fulano@teste.com" && a.Succeeded),
            It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokens.Verify(r => r.AddAsync(
            It.Is<RefreshToken>(t => t.UserId == userId && t.TokenHash == "refresh-token-hash"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
