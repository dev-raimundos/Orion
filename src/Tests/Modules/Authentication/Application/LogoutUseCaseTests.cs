using Authentication.Application.Abstractions;
using Authentication.Application.UseCases.Logout;
using Authentication.Domain;
using Authentication.Domain.Abstractions;
using Moq;

namespace Authentication.Tests.Application;

public class LogoutUseCaseTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokens = new();
    private readonly Mock<IRefreshTokenGenerator> _refreshTokenGenerator = new();

    private LogoutUseCase CreateSut() => new(_refreshTokens.Object, _refreshTokenGenerator.Object);

    [Fact]
    public async Task ExecuteAsync_WhenTokenDoesNotExist_DoesNothing()
    {
        _refreshTokenGenerator.Setup(g => g.Hash("token-desconhecido")).Returns("hash-desconhecido");
        _refreshTokens.Setup(r => r.GetByTokenHashAsync("hash-desconhecido", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var sut = CreateSut();

        await sut.ExecuteAsync(new LogoutRequest("token-desconhecido"), CancellationToken.None);

        _refreshTokens.Verify(r => r.UpdateAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTokenExists_RevokesAndPersists()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "fulano@teste.com", "hash", TimeSpan.FromDays(7));

        _refreshTokenGenerator.Setup(g => g.Hash("token-atual")).Returns("hash");
        _refreshTokens.Setup(r => r.GetByTokenHashAsync("hash", It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var sut = CreateSut();

        await sut.ExecuteAsync(new LogoutRequest("token-atual"), CancellationToken.None);

        Assert.False(token.IsActive);
        _refreshTokens.Verify(r => r.UpdateAsync(token, It.IsAny<CancellationToken>()), Times.Once);
    }
}
