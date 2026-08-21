using Api.Modules.Users.Infrastructure.Security;

namespace Users.Infrastructure;

public class PasswordHasherTests
{
    private readonly PasswordHasher _sut = new();

    [Fact]
    public void Hash_ThenVerify_WithCorrectPassword_ReturnsTrue()
    {
        var hash = _sut.Hash("minha-senha");

        Assert.True(_sut.Verify("minha-senha", hash));
    }

    [Fact]
    public void Verify_WithWrongPassword_ReturnsFalse()
    {
        var hash = _sut.Hash("minha-senha");

        Assert.False(_sut.Verify("senha-errada", hash));
    }

    [Fact]
    public void Hash_CalledTwiceForSamePassword_ProducesDifferentHashes()
    {
        var hash1 = _sut.Hash("minha-senha");
        var hash2 = _sut.Hash("minha-senha");

        Assert.NotEqual(hash1, hash2);
    }
}
