using Microsoft.EntityFrameworkCore;
using Users.Domain;

namespace Users.Infrastructure;

public class UserRepository(UsersDbContext context) : IUserRepository
{
    private readonly UsersDbContext _context = context;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);
    }
}