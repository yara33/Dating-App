using System;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string pUsername, string pPwd)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == pUsername);

            if (user == null)
                return null;

            if (!VerifiedPasswordHash(pPwd, user.PasswordHash, user.PasswordSalt))
                return null;
            
            return user;
        }

        private bool VerifiedPasswordHash(string pPwd, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pPwd));

                for (int i = 0;i < computeHash.Length; i++)
                {
                    if (computeHash[i] != passwordHash[i])
                        return false;
                }
                return true;
            }
        }

        public async Task<User> Register(User pUser, string pPwd)
        {
            byte[] pwdHash, pwdSalt;
            CreatePasswordHash(pPwd, out pwdHash, out pwdSalt);
            pUser.PasswordHash = pwdHash;
            pUser.PasswordSalt = pwdSalt;

            await _context.Users.AddAsync(pUser);
            await _context.SaveChangesAsync();

            return pUser;
        }

        private void CreatePasswordHash(string pPwd, out byte[] pwdHash, out byte[] pwdSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pwdSalt = hmac.Key;
                pwdHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pPwd));
            }
        }

        public async Task<bool> UserExists(string pUsername)
        {
            if (await _context.Users.AnyAsync(u => u.Username == pUsername))
                return true;
            
            return false;
        }
    }
}