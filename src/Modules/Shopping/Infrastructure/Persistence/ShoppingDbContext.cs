using Microsoft.EntityFrameworkCore;

using Shopping.Domain;

namespace Shopping.Infrastructure.Persistence;

public class ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("Item", "shopping");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name).HasMaxLength(200);
            entity.Property(u => u.Description).HasMaxLength(355);
            entity.Property(u => u.Url);
        });
    }
}