using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User pUser, string pPwd);
         Task<User> Login(string pUsername, string pPwd);
         Task<bool> UserExists(string pUsername);
    }
}