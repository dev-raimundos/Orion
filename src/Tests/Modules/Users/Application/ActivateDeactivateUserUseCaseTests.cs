using Api.Common;
using Api.Modules.Users.Application.UseCases;
using Api.Modules.Users.Domain;
using Api.Modules.Users.Domain.Abstractions;
using Moq;

namespace Users.Application;

public class ActivateDeactivateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repository = new();

    [Fact]
    public async Task Activate_WhenUserDoesNotExist_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = new ActivateUserUseCase(_repository.Object);

        await Assert.ThrowsAsync<AppNotFoundException>(() =>
            sut.ExecuteAsync(new ActivateUserRequest(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Activate_WhenUserIsInactive_ActivatesAndPersists()
    {
        var user = User.Create("Nome", "email@teste.com", "hash");
        user.Deactivate();
        _repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut = new ActivateUserUseCase(_repository.Object);
        var result = await sut.ExecuteAsync(new ActivateUserRequest(user.Id), CancellationToken.None);

        Assert.True(result.Active);
        _repository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Deactivate_WhenUserDoesNotExist_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = new DeactivateUserUseCase(_repository.Object);

        await Assert.ThrowsAsync<AppNotFoundException>(() =>
            sut.ExecuteAsync(new DeactivateUserRequest(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Deactivate_WhenUserIsActive_DeactivatesAndPersists()
    {
        var user = User.Create("Nome", "email@teste.com", "hash");
        _repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut = new DeactivateUserUseCase(_repository.Object);
        var result = await sut.ExecuteAsync(new DeactivateUserRequest(user.Id), CancellationToken.None);

        Assert.False(result.Active);
        _repository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
