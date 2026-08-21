using Microsoft.EntityFrameworkCore;
using Authentication.Domain;

namespace Authentication.Infrastructure.Persistence;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : DbContext(options)
{
    public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoginAttempt>(entity =>
        {
            entity.ToTable("LoginAttempts", schema: "auth");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Email).IsRequired().HasMaxLength(320);

            entity.HasIndex(a => new { a.Email, a.AttemptedAt });
        });
    }
}
