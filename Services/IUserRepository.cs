using SecretSantaApp.Models;
using System.Collections.Generic;

namespace SecretSantaApp.Services
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();
        bool SaveUser(User user);
        bool DeleteUser(int id);
        bool UpdateUser(int id, string name);
        User GetUserByGuid(int id);
        Dictionary<string, string> RunMatchingAlgorithm();
    }
}