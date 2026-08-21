using Orion.Application;
using Users.Application.UseCases;
using Users.Domain;
using Users.Domain.Abstractions;
using Moq;

namespace Users.Tests.Application;

public class VerifyEmailUseCaseTests
{
    private readonly Mock<IUserRepository> _repository = new();

    private VerifyEmailUseCase CreateSut() => new(_repository.Object);

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = CreateSut();

        await Assert.ThrowsAsync<AppNotFoundException>(() =>
            sut.ExecuteAsync(new VerifyEmailRequest(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserExists_VerifiesEmailAndPersists()
    {
        var user = User.Create("Nome", "email@teste.com", "hash");
        _repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut = CreateSut();
        var result = await sut.ExecuteAsync(new VerifyEmailRequest(user.Id), CancellationToken.None);

        Assert.True(result.EmailVerified);
        _repository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
