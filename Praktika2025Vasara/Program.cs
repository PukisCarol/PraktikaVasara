using Praktika2025Vasara.Entities;
using Praktika2025Vasara.Services;
using Praktika2025Vasara.Store;

namespace Praktika2025Vasara;

internal class Program
{
    static void Main(string[] args)
    {
        IFileStore<User> userStore = new SimpleFileStore<User>("../../../Data/users.json");
        IFileStore<Shortage> shortageStore = new SimpleFileStore<Shortage>("../../../Data/shortages.json");

        IUserService users = new UserService(userStore);
        IShortageService shortages = new ShortageService(shortageStore);

        ConsoleController controller = new ConsoleController(users, shortages);
        controller.Run();
    }
}
