using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class DeleteUserView
{
    private readonly IUserRepository userRepository;
    
    public DeleteUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    
    public async Task ShowAsync(int userId)
    {
        await userRepository.DeleteAsync(userId);
        Console.WriteLine($"User with ID {userId} deleted.");
    }
}