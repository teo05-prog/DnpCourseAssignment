using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository userRepository;

    public ListUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
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
            Console.WriteLine($"{user.Id,-5} {user.UserName,-20}");
        }
    }
}