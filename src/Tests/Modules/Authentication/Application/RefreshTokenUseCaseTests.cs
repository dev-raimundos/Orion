using Orion.SharedKernel;
using Authentication.Application.Abstractions;
using Authentication.Application.UseCases;
using Authentication.Domain;
using Authentication.Domain.Abstractions;
using Moq;

namespace Authentication.Tests.Application;

public class RefreshTokenUseCaseTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokens = new();
    private readonly Mock<IRefreshTokenGenerator> _refreshTokenGenerator = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();

    private RefreshTokenUseCase CreateSut() => new(_refreshTokens.Object, _refreshTokenGenerator.Object, _tokenGenerator.Object);

    [Fact]
    public async Task ExecuteAsync_WhenTokenDoesNotExist_ThrowsUnauthorized()
    {
        _refreshTokenGenerator.Setup(g => g.Hash("token-desconhecido")).Returns("hash-desconhecido");
        _refreshTokens.Setup(r => r.GetByTokenHashAsync("hash-desconhecido", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var sut = CreateSut();

        await Assert.ThrowsAsync<AppUnauthorizedException>(() =>
            sut.ExecuteAsync(new RefreshTokenRequest("token-desconhecido"), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WhenTokenIsAlreadyRevoked_ThrowsUnauthorized()
    {
        var revoked = RefreshToken.Create(Guid.NewGuid(), "fulano@teste.com", "hash", TimeSpan.FromDays(7));
        revoked.Revoke();

        _refreshTokenGenerator.Setup(g => g.Hash("token-revogado")).Returns("hash");
        _refreshTokens.Setup(r => r.GetByTokenHashAsync("hash", It.IsAny<CancellationToken>())).ReturnsAsync(revoked);

        var sut = CreateSut();

        await Assert.ThrowsAsync<AppUnauthorizedException>(() =>
            sut.ExecuteAsync(new RefreshTokenRequest("token-revogado"), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WhenTokenIsExpired_ThrowsUnauthorized()
    {
        var expired = RefreshToken.Create(Guid.NewGuid(), "fulano@teste.com", "hash", TimeSpan.FromSeconds(-1));

        _refreshTokenGenerator.Setup(g => g.Hash("token-expirado")).Returns("hash");
        _refreshTokens.Setup(r => r.GetByTokenHashAsync("hash", It.IsAny<CancellationToken>())).ReturnsAsync(expired);

        var sut = CreateSut();

        await Assert.ThrowsAsync<AppUnauthorizedException>(() =>
            sut.ExecuteAsync(new RefreshTokenRequest("token-expirado"), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WhenTokenIsValid_RevokesTheOldTokenAndPersistsANewOne()
    {
        var userId = Guid.NewGuid();
        var current = RefreshToken.Create(userId, "fulano@teste.com", "hash-atual", TimeSpan.FromDays(7));
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(15);

        _refreshTokenGenerator.Setup(g => g.Hash("token-atual")).Returns("hash-atual");
        _refreshTokens.Setup(r => r.GetByTokenHashAsync("hash-atual", It.IsAny<CancellationToken>())).ReturnsAsync(current);
        _refreshTokenGenerator.Setup(g => g.Generate()).Returns(("token-novo", "hash-novo"));
        _tokenGenerator.Setup(t => t.Generate(userId, "fulano@teste.com")).Returns(("access-token-novo", expiresAt));

        var sut = CreateSut();
        var result = await sut.ExecuteAsync(new RefreshTokenRequest("token-atual"), CancellationToken.None);

        Assert.False(current.IsActive);
        Assert.Equal("access-token-novo", result.AccessToken);
        Assert.Equal("token-novo", result.RefreshToken);
        Assert.NotEqual("token-atual", result.RefreshToken);

        _refreshTokens.Verify(r => r.UpdateAsync(current, It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokens.Verify(r => r.AddAsync(
            It.Is<RefreshToken>(t => t.UserId == userId && t.TokenHash == "hash-novo"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
