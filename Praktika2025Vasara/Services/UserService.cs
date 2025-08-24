using Praktika2025Vasara.Entities;
using Praktika2025Vasara.Store;

namespace Praktika2025Vasara.Services;

public class UserService : IUserService
{
    private readonly IFileStore<User> _store;

    public UserService(IFileStore<User> store)
    {
        _store = store;
    }

    public bool IsUserValid(User user)
    {
        List<User> users = _store.GetItems();
        if (users.Any(u => u.UserName == user.UserName && u.Password == user.Password))
        {
            return true;
        }
        return false;
    }

    public User GetUserByCredentials(string username, string password)
    {
        List<User> users = _store.GetItems();
        return users.FirstOrDefault(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)
                                         && u.Password == password);
    }

    public void AddNewUser(User user)
    {
        List<User> users = _store.GetItems();
        if (users.Any(u => u.UserName == user.UserName))
        {
            throw new InvalidOperationException("User already exists!");
        }

        users.Add(user);

        _store.SaveItems(users);
    }
}
