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
        Console.Clear();
        Console.WriteLine("=== Create New User ===");

        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(username))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        // Check if username already exists
        var existingUsers = userRepository.GetManyAsync()
            .Where(u => u.Username == username);
        if (existingUsers.Any())
        {
            Console.WriteLine(
                "Username already exists. Please choose a different username.");
            return;
        }

        Console.Write("Password: ");
        var password = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        var user = new User
        {
            Username = username,
            Password = password
        };

        var created = await userRepository.AddAsync(user);
        Console.WriteLine($"User created successfully with ID: {created.Id}");
    }

    private async Task ListUsersAsync()
    {
        Console.Clear();
        Console.WriteLine("=== All Users ===");

        var users = userRepository.GetManyAsync().ToList();
        if (!users.Any())
        {
            Console.WriteLine("No users found.");
            return;
        }

        Console.WriteLine($"{"ID",-5} {"Username",-20}");
        Console.WriteLine(new string('-', 30));
        foreach (var user in users)
        {
            Console.WriteLine($"{user.Id,-5} {user.Username,-20}");
        }
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

        try
        {
            var user = await userRepository.GetSingleAsync(id);
            Console.WriteLine($"ID: {user.Id}");
            Console.WriteLine($"Username: {user.Username}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
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

        try
        {
            var user = await userRepository.GetSingleAsync(id);
            Console.WriteLine($"Current Username: {user.Username}");

            Console.Write("New Username (press Enter to keep current): ");
            var newUsername = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(newUsername))
            {
                // Check if new username already exists
                var existingUsers = userRepository.GetManyAsync()
                    .Where(u => u.Username == newUsername && u.Id != id);
                if (existingUsers.Any())
                {
                    Console.WriteLine(
                        "Username already exists. Please choose a different username.");
                    return;
                }

                user.Username = newUsername;
            }

            Console.Write("New Password (press Enter to keep current): ");
            var newPassword = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(newPassword))
            {
                user.Password = newPassword;
            }

            await userRepository.UpdateAsync(user);
            Console.WriteLine("User updated successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
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

        try
        {
            var user = await userRepository.GetSingleAsync(id);
            Console.WriteLine(
                $"Are you sure you want to delete user '{user.Username}'? (y/n): ");
            var confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "y" || confirmation == "yes")
            {
                await userRepository.DeleteAsync(id);
                Console.WriteLine("User deleted successfully.");
            }
            else
            {
                Console.WriteLine("Delete operation cancelled.");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}