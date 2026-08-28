using Orion.SharedKernel;
using Users.Application.UseCases.GetUserById;
using Users.Domain;
using Users.Domain.Abstractions;
using Moq;

namespace Users.Tests.Application;

public class GetUserByIdUseCaseTests
{
    private readonly Mock<IUserRepository> _repository = new();

    private GetUserByIdUseCase CreateSut() => new(_repository.Object);

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ThrowsNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = CreateSut();

        await Assert.ThrowsAsync<AppNotFoundException>(() =>
            sut.ExecuteAsync(new GetUserByIdInput(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserExists_ReturnsMappedResponse()
    {
        var user = new User("Nome", "email@teste.com", "hash");
        _repository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut = CreateSut();
        var result = await sut.ExecuteAsync(new GetUserByIdInput(user.Id), CancellationToken.None);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Active, result.Active);
        Assert.Equal(user.EmailVerified, result.EmailVerified);
    }
}
