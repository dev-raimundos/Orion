using Orion.Application;
using Users.Application.UseCases;
using Users.Domain;
using Users.Domain.Abstractions;
using Moq;

namespace Users.Tests.Application;

public class RenameUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repository = new();

    private RenameUserUseCase CreateSut() => new(_repository.Object);

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = CreateSut();

        await Assert.ThrowsAsync<AppNotFoundException>(() =>
            sut.ExecuteAsync(new RenameUserRequest(Guid.NewGuid(), "Novo Nome"), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserExists_RenamesAndPersists()
    {
        var user = User.Create("Nome Antigo", "email@teste.com", "hash");
        _repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut = CreateSut();
        var result = await sut.ExecuteAsync(new RenameUserRequest(user.Id, "Nome Novo"), CancellationToken.None);

        Assert.Equal("Nome Novo", result.Name);
        _repository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
