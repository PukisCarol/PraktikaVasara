namespace Praktika2025Vasara.Entities;

public class User
{
    public User(string userName, string password)
    {
        UserName = userName;
        Password = password;
        Role = Role.User;
    }

    public string UserName { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
}
