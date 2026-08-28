using Orion.SharedKernel;
using Users.Application.Abstractions;
using Users.Application.UseCases.CreateUser;
using Users.Domain;
using Users.Domain.Abstractions;
using Moq;

namespace Users.Tests.Application;

public class CreateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    private CreateUserUseCase CreateSut() => new(_repository.Object, _passwordHasher.Object);

    [Fact]
    public async Task ExecuteAsync_WhenEmailAlreadyExists_ThrowsConflict()
    {
        _repository.Setup(r => r.GetByEmailAsync("fulano@teste.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User("Fulano", "fulano@teste.com", "hash"));

        var sut = CreateSut();
        var request = new CreateUserInput("Fulano", "fulano@teste.com", "senha123");

        await Assert.ThrowsAsync<AppConflictException>(() => sut.ExecuteAsync(request, CancellationToken.None));

        _repository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailIsFree_CreatesAndPersistsUser()
    {
        _repository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasher.Setup(h => h.Hash("senha123")).Returns("hash-gerado");

        var sut = CreateSut();
        var request = new CreateUserInput("Fulano", "fulano@teste.com", "senha123");

        var result = await sut.ExecuteAsync(request, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        _repository.Verify(r => r.AddAsync(
            It.Is<User>(u => u.Email == "fulano@teste.com" && u.PasswordHash == "hash-gerado"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
