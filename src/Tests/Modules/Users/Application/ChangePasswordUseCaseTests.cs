using Api.Common;
using Api.Modules.Users.Application.Abstractions;
using Api.Modules.Users.Application.UseCases;
using Api.Modules.Users.Domain;
using Api.Modules.Users.Domain.Abstractions;
using Moq;

namespace Users.Application;

public class ChangePasswordUseCaseTests
{
    private readonly Mock<IUserRepository> _repository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    private ChangePasswordUseCase CreateSut() => new(_repository.Object, _passwordHasher.Object);

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = CreateSut();

        await Assert.ThrowsAsync<AppNotFoundException>(() =>
            sut.ExecuteAsync(new ChangePasswordRequest(Guid.NewGuid(), "atual", "nova"), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WhenCurrentPasswordIsInvalid_ThrowsUnauthorized()
    {
        var user = User.Create("Nome", "email@teste.com", "hash-atual");
        _repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("senha-errada", "hash-atual")).Returns(false);

        var sut = CreateSut();

        await Assert.ThrowsAsync<AppUnauthorizedException>(() =>
            sut.ExecuteAsync(new ChangePasswordRequest(user.Id, "senha-errada", "nova"), CancellationToken.None));

        _repository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCurrentPasswordIsValid_ChangesAndPersists()
    {
        var user = User.Create("Nome", "email@teste.com", "hash-atual");
        _repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("senha-certa", "hash-atual")).Returns(true);
        _passwordHasher.Setup(h => h.Hash("nova-senha")).Returns("hash-novo");

        var sut = CreateSut();
        var result = await sut.ExecuteAsync(new ChangePasswordRequest(user.Id, "senha-certa", "nova-senha"), CancellationToken.None);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal("hash-novo", user.PasswordHash);
        _repository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
