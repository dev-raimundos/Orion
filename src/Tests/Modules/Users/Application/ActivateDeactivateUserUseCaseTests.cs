using Orion.SharedKernel;
using Users.Application.UseCases.ActivateUser;
using Users.Application.UseCases.DeactivateUser;
using Users.Domain;
using Users.Domain.Abstractions;
using Moq;

namespace Users.Tests.Application;

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
            sut.ExecuteAsync(new ActivateUserInput(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Activate_WhenUserIsInactive_ActivatesAndPersists()
    {
        var user = new User("Nome", "email@teste.com", "hash");
        user.Deactivate();
        _repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut = new ActivateUserUseCase(_repository.Object);
        var result = await sut.ExecuteAsync(new ActivateUserInput(user.Id), CancellationToken.None);

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
            sut.ExecuteAsync(new DeactivateUserInput(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Deactivate_WhenUserIsActive_DeactivatesAndPersists()
    {
        var user = new User("Nome", "email@teste.com", "hash");
        _repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut = new DeactivateUserUseCase(_repository.Object);
        var result = await sut.ExecuteAsync(new DeactivateUserInput(user.Id), CancellationToken.None);

        Assert.False(result.Active);
        _repository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
