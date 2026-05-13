using FlowBoard.AuthService.Data;
using FlowBoard.AuthService.Entities;
using FlowBoard.AuthService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.AuthService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _db;
    public UserRepository(AuthDbContext db) => _db = db;

    public Task<User?> FindByEmailAsync(string email)
        => _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> FindByUsernameAsync(string username)
        => _db.Users.FirstOrDefaultAsync(u => u.Username == username);

    public Task<User?> FindByIdAsync(int userId)
        => _db.Users.FindAsync(userId).AsTask();

    public Task<bool> ExistsByEmailAsync(string email)
        => _db.Users.AnyAsync(u => u.Email == email);

    public Task<bool> ExistsByUsernameAsync(string username)
        => _db.Users.AnyAsync(u => u.Username == username);

    public Task<List<User>> FindAllByRoleAsync(string role)
        => _db.Users.Where(u => u.Role == role).ToListAsync();

    public Task<List<User>> SearchByFullNameAsync(string q)
        => _db.Users
              .Where(u => u.FullName.Contains(q) || u.Username.Contains(q))
              .Take(20).ToListAsync();

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(int userId)
    {
        var user = await FindByIdAsync(userId);
        if (user != null) { _db.Users.Remove(user); await _db.SaveChangesAsync(); }
    }
}