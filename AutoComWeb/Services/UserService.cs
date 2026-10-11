using System.Security.Cryptography;
using System.Text;
using AutoComWeb.Data;
using AutoComWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoComWeb.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateAsync(string phone, string password)
    {
        var user = await _context.Users
            .Include(u => u.Client)
            .FirstOrDefaultAsync(u => u.Phone == phone);

        if (user == null)
            return null;

        var hashedPassword = HashPassword(password);
        if (user.Passwordhash != hashedPassword)
            return null;

        return user;
    }

public async Task<User> RegisterAsync(string phone, string password, string role, string firstName, string lastName)
{
    var existingUser = await _context.Users
        .FirstOrDefaultAsync(u => u.Phone == phone);

    if (existingUser != null)
        throw new InvalidOperationException("User with this phone already exists");

    var user = new User
    {
        Phone = phone,
        Passwordhash = HashPassword(password),
        Role = role
    };

    var client = new Client
    {
        FirstName = firstName,
        LastName = lastName,
        Phone = phone,
        User = user
    };

    _context.Clients.Add(client);
    await _context.SaveChangesAsync();

    return user;
}

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}