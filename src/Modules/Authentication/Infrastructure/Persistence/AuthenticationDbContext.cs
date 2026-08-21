using Microsoft.EntityFrameworkCore;
using Authentication.Domain;

namespace Authentication.Infrastructure.Persistence;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : DbContext(options)
{
    public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoginAttempt>(entity =>
        {
            entity.ToTable("LoginAttempts", schema: "auth");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Email).IsRequired().HasMaxLength(320);

            entity.HasIndex(a => new { a.Email, a.AttemptedAt });
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens", schema: "auth");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Email).IsRequired().HasMaxLength(320);
            entity.Property(t => t.TokenHash).IsRequired().HasMaxLength(512);

            entity.HasIndex(t => t.TokenHash).IsUnique();
            entity.HasIndex(t => t.UserId);
        });
    }
}
