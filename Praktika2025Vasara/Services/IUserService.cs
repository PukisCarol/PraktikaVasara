using Praktika2025Vasara.Entities;

namespace Praktika2025Vasara.Services;

public interface IUserService
{
    bool IsUserValid(User user);
    User GetUserByCredentials(string username, string password);
    void AddNewUser(User user);
}