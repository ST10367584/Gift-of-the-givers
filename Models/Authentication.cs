using Gift_of_the_givers.Models;
using Gift_of_the_givers.Data;

using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Gift_of_the_givers.Services
{
    public interface IAuthService
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> Register(User user, string password)
        {
            if (await UserExists(user.Username))
                throw new Exception("Username already exists");

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = Convert.ToBase64String(passwordHash);
            user.PasswordSalt = Convert.ToBase64String(passwordSalt);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null || !VerifyPasswordHash(password,
                Convert.FromBase64String(user.PasswordHash),
                Convert.FromBase64String(user.PasswordSalt)))
                return null;

            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}