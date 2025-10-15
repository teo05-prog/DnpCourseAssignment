using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
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

        var existingUsers = userRepository.GetManyAsync()
            .Where(u => u.UserName == username);
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
            UserName = username,
            Password = password
        };

        var created = await userRepository.AddAsync(user);
        Console.WriteLine($"User created successfully with ID: {created.Id}");
    }
}