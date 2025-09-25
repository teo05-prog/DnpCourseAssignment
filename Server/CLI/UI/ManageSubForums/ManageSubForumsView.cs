using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageSubForums;

public class ManageSubForumsView
{
    private readonly ISubForumRepository subForumRepository;
    private readonly IUserRepository userRepository;

    public ManageSubForumsView(ISubForumRepository subForumRepository, IUserRepository userRepository)
    {
        this.subForumRepository = subForumRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            ShowSubForumMenu();
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    await CreateSubForumAsync();
                    break;
                case "2":
                    await ListSubForumsAsync();
                    break;
                case "3":
                    await ViewSubForumAsync();
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

    private void ShowSubForumMenu()
    {
        Console.Clear();
        Console.WriteLine("=== SubForum Management ===");
        Console.WriteLine("1. Create SubForum");
        Console.WriteLine("2. List All SubForums");
        Console.WriteLine("3. View SubForum");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Choose an option: ");
    }

    private async Task CreateSubForumAsync()
    {
        var createView = new CreateSubForumView(subForumRepository);
        await createView.ShowAsync();
    }

    private async Task ListSubForumsAsync()
    {
        var listView = new ListSubForumsView(subForumRepository);
        await listView.Show();
    }

    private async Task ViewSubForumAsync()
    {
        var singleView = new SingleSubForumView(subForumRepository);
        await singleView.ShowAsync();
    }
}