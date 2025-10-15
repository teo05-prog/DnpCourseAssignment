using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class UpdateUserView
{
    private readonly IUserRepository userRepository;

    public UpdateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync(int userId)
    {
        var user = await userRepository.GetSingleAsync(userId);
        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        Console.WriteLine($"Current Username: {user.UserName}");
        Console.Write("New Username: ");
        var newUsername = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(newUsername))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        Console.WriteLine($"Current Password: {user.Password}");
        Console.Write("New Password: ");
        var newPassword = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(newPassword))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        user.UserName = newUsername;
        user.Password = newPassword;
        await userRepository.UpdateAsync(user);
        Console.WriteLine("User updated successfully.");
    }
}