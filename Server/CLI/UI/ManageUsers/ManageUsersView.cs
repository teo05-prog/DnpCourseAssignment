using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ManageUsersView
{
    private readonly IUserRepository userRepository;

    public ManageUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            ShowUserMenu();
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    await CreateUserAsync();
                    break;
                case "2":
                    await ListUsersAsync();
                    break;
                case "3":
                    await ViewUserAsync();
                    break;
                case "4":
                    await UpdateUserAsync();
                    break;
                case "5":
                    await DeleteUserAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    private void ShowUserMenu()
    {
        Console.Clear();
        Console.WriteLine("=== User Management ===");
        Console.WriteLine("1. Create User");
        Console.WriteLine("2. List All Users");
        Console.WriteLine("3. View User");
        Console.WriteLine("4. Update User");
        Console.WriteLine("5. Delete User");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Choose an option: ");
    }

    private async Task CreateUserAsync()
    {
        var createUserView = new CreateUserView(userRepository);
        await createUserView.ShowAsync();
    }

    private async Task ListUsersAsync()
    {
        var listUsersView = new ListUsersView(userRepository);
        await listUsersView.ShowAsync();
    }

    private async Task ViewUserAsync()
    {
        Console.Clear();
        Console.WriteLine("=== View User ===");

        Console.Write("Enter User ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        var singleUserView = new SingleUserView(userRepository);
        await singleUserView.ShowAsync(id);
    }

    private async Task UpdateUserAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Update User ===");

        Console.Write("Enter User ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        var updateUserView = new UpdateUserView(userRepository);
        await updateUserView.ShowAsync(id);
    }

    private async Task DeleteUserAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Delete User ===");

        Console.Write("Enter User ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        var deleteUserView = new DeleteUserView(userRepository);
        await deleteUserView.ShowAsync(id);
    }
}