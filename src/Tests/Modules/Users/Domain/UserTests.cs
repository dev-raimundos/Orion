using Users.Domain;

namespace Users.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesActiveUnverifiedUser()
    {
        var user = new User("Fulano", "fulano@teste.com", "hash");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Fulano", user.Name);
        Assert.Equal("fulano@teste.com", user.Email);
        Assert.Equal("hash", user.PasswordHash);
        Assert.True(user.Active);
        Assert.False(user.EmailVerified);
        Assert.Null(user.LastLoginAt);
    }

    [Theory]
    [InlineData("", "email@teste.com", "hash")]
    [InlineData("Nome", "", "hash")]
    [InlineData("Nome", "email@teste.com", "")]
    public void Constructor_WithMissingRequiredField_Throws(string name, string email, string passwordHash)
    {
        Assert.Throws<ArgumentException>(() => new User(name, email, passwordHash));
    }

    [Fact]
    public void Rename_WithValidName_UpdatesNameAndTouchesUpdatedAt()
    {
        var user = new User("Nome Antigo", "email@teste.com", "hash");
        var updatedAtBefore = user.UpdatedAt;

        user.Rename("Nome Novo");

        Assert.Equal("Nome Novo", user.Name);
        Assert.True(user.UpdatedAt >= updatedAtBefore);
    }

    [Fact]
    public void Rename_WithEmptyName_Throws()
    {
        var user = new User("Nome", "email@teste.com", "hash");

        Assert.Throws<ArgumentException>(() => user.Rename(" "));
    }

    [Fact]
    public void ChangePassword_WithValidHash_UpdatesPasswordHash()
    {
        var user = new User("Nome", "email@teste.com", "hash-antigo");

        user.ChangePassword("hash-novo");

        Assert.Equal("hash-novo", user.PasswordHash);
    }

    [Fact]
    public void VerifyEmail_WhenNotVerified_MarksAsVerified()
    {
        var user = new User("Nome", "email@teste.com", "hash");

        user.VerifyEmail();

        Assert.True(user.EmailVerified);
    }

    [Fact]
    public void Deactivate_ThenActivate_TogglesActiveFlag()
    {
        var user = new User("Nome", "email@teste.com", "hash");

        user.Deactivate();
        Assert.False(user.Active);

        user.Activate();
        Assert.True(user.Active);
    }

    [Fact]
    public void RegisterLogin_SetsLastLoginAt()
    {
        var user = new User("Nome", "email@teste.com", "hash");

        user.RegisterLogin();

        Assert.NotNull(user.LastLoginAt);
    }
}
