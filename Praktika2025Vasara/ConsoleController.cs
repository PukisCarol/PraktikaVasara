using Praktika2025Vasara.Entities;
using Praktika2025Vasara.Services;

namespace Praktika2025Vasara;

 public class ConsoleController
{
    private readonly IUserService _userService;
    private readonly IShortageService _shortageService;

    public ConsoleController(IUserService userService, IShortageService shortageService)
    {
        _userService = userService;
        _shortageService = shortageService;
    }
    public void Run()
    {
        bool exit = false;
        while (!exit)
        {
            PrintMainMenu();
            string input = Console.ReadLine();
            if (input != null && input.Length == 1 && input.All(char.IsDigit))
            {
                if (input[0] == '1')
                {
                    RunLogin();
                }
                if (input[0] == '2')
                {
                    RunRegistration();
                }
                if (input[0] == '3')
                {
                    exit = true;
                }
            }
            else
            {
                Console.WriteLine("Wrong input!");
            }

        }
    }
    private void PrintMainMenu()
    {
        Console.WriteLine("===============================");
        Console.WriteLine("Hello, welcome to shortages manager, choose options by writing number:");
        Console.WriteLine("Login(1)");
        Console.WriteLine("Register(2)");
        Console.WriteLine("Exit(3)");
        Console.WriteLine("===============================");
        Console.Write("Choose option: ");
    }

    private void RunLogin()
    {
        Console.WriteLine("\n--- Login ---");
        Console.WriteLine("You are trying to login, write username and password");
        User inputtedUser = ReadUser();
        User user = _userService.GetUserByCredentials(inputtedUser.UserName, inputtedUser.Password);

        if (user != null)
        {
            Console.WriteLine($"\nWelcome, {user.UserName} ({user.Role})!");
            RunLoggedInUserTasks(user);
        }
        else
        {
            Console.WriteLine("User not found or wrong input");
        }
    }

    private void RunRegistration()
    {
        Console.WriteLine("\n--- Register ---");
        Console.WriteLine("You are trying to register, write username and password");
        User newUser = ReadUser();

        if (!_userService.IsUserValid(newUser))
        {
            _userService.AddNewUser(newUser);
            Console.WriteLine("User created successfully! You can now log in!\n");
        }
        else
        {
            Console.WriteLine("User already exists.\n");
        }
    }

    private User ReadUser()
    {
        Console.Write("Username: ");
        string loginName = Console.ReadLine();
        Console.Write("Password: ");
        string loginPassword = Console.ReadLine();
        User user = new User(loginName, loginPassword);
        return user;
    }

    private void RunLoggedInUserTasks(User user)
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("List all shortages(1)");
            Console.WriteLine("Add new shortage(2)");
            Console.WriteLine("Delete shortage(3)");
            Console.WriteLine("Log out(4)");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    ListShortages(user);
                    break;

                case "2":
                    AddShortage(user);
                    break;

                case "3":
                    DeleteShortage(user);
                    break;

                case "4":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Wrong input!");
                    break;
            }
        }
    }

    private void ListShortages(User user)
    {
        Console.WriteLine("Do you want to add filters? (y/n)");
        string filterChoice = Console.ReadLine();

        string titleFilter = null;
        DateTime? fromDate = null, toDate = null;
        Category? categoryFilter = null;
        Room? roomFilter = null;

        if (filterChoice != null && filterChoice.ToLower() == "y")
        {
            Console.Write("Filter by title (or leave empty): ");
            string titleInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(titleInput))
            {
                titleFilter = titleInput;
            }

            Console.Write("Filter by start date (yyyy-MM-dd) or leave empty: ");
            string fromInput = Console.ReadLine();
            if (DateTime.TryParse(fromInput, out DateTime fDate))
            {
                fromDate = fDate;
            }

            Console.Write("Filter by end date (yyyy-MM-dd) or leave empty: ");
            string toInput = Console.ReadLine();
            if (DateTime.TryParse(toInput, out DateTime tDate))
            {
                toDate = tDate.Date.AddDays(1).AddTicks(-1);
            }

            Console.Write("Filter by category (1-Electronics, 2-Food, 3-Other, empty = skip): ");
            string catInput = Console.ReadLine();
            if (int.TryParse(catInput, out int catVal) && catVal >= 1 && catVal <= 3)
            {
                categoryFilter = (Category)(catVal - 1);
            }

            Console.Write("Filter by room (1-MeetingRoom, 2-Kitchen, 3-Bathroom, empty = skip): ");
            string roomInput = Console.ReadLine();
            if (int.TryParse(roomInput, out int roomVal) && roomVal >= 1 && roomVal <= 3)
            {
                roomFilter = (Room)(roomVal - 1);
            }
        }

        bool isAdmin = user.Role == Role.Admin;
        List<Shortage> shortages = _shortageService.FindShortages(user.UserName, isAdmin, titleFilter, fromDate, toDate, categoryFilter, roomFilter);

        PrintShortages(shortages);
    }


    private void AddShortage(User user)
    {
        Console.Write("Enter title: ");
        string title = Console.ReadLine();
        Console.Write("Enter description/name: ");
        string name = Console.ReadLine();

        Console.WriteLine("Choose room: 1-MeetingRoom, 2-Kitchen, 3-Bathroom");
        Room room = (Room)(int.Parse(Console.ReadLine()) - 1);

        Console.WriteLine("Choose category: 1-Electronics, 2-Food, 3-Other");
        Category category = (Category)(int.Parse(Console.ReadLine()) - 1);

        Console.Write("Enter priority (1-10): ");
        int? priority = int.Parse(Console.ReadLine());

        Shortage shortage = new Shortage(title, name, room, category, priority, user.UserName);
        if(_shortageService.DoesShortageExists(shortage))
        {
            Console.WriteLine("Existing shortage updated with higher priority.");
        }
        else
        {
            _shortageService.AddShortage(shortage);
            Console.WriteLine("Shortage added successfully!");
        }
    }

    private void DeleteShortage(User user)
    {
        Console.Write("Enter title to delete: ");
        string delTitle = Console.ReadLine();
        Console.WriteLine("Choose room: 1-MeetingRoom, 2-Kitchen, 3-Bathroom");
        Room delRoom = (Room)(int.Parse(Console.ReadLine()) - 1);

        if(_shortageService.DeleteShortage(delTitle, delRoom, user.UserName, user.Role == Role.Admin))
        {
            Console.WriteLine("Shortage deleted successfully!");
        }
    }

    private void PrintShortages(List<Shortage> shortages)
    {
        if (shortages == null || shortages.Count == 0)
        {
            Console.WriteLine("No shortages found.");
            return;
        }

        Console.WriteLine("\n--- Shortages ---");

        foreach (Shortage shortage in shortages)
        {
            Console.WriteLine("────────────────────────────");
            Console.WriteLine($" Title      : {shortage.Title}");
            Console.WriteLine($" Description: {shortage.Name}");
            Console.WriteLine($" Room       : {shortage.Room}");
            Console.WriteLine($" Category   : {shortage.Category}");
            Console.WriteLine($" Priority   : {shortage.Priority}/10");
            Console.WriteLine($" Created by : {shortage.CreatedBy}");
            Console.WriteLine($" Created on : {shortage.CreatedOn:yyyy-MM-dd HH:mm}");
        }

        Console.WriteLine("────────────────────────────\n");
    }
}
