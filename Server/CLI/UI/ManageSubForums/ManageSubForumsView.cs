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
        Console.Clear();
        Console.WriteLine("=== Create New SubForum ===");

        Console.Write("SubForum Name: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }

        Console.Write("Description: ");
        var description = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(description))
        {
            Console.WriteLine("Description cannot be empty.");
            return;
        }

        Console.Write("Creator User ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid User ID format.");
            return;
        }

        // Validate user exists
        try
        {
            await userRepository.GetSingleAsync(userId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("User with specified ID does not exist.");
            return;
        }

        var subForum = new SubForum
        {
            Name = name,
            Description = description,
            CreatorUserId = userId
        };

        var created = await subForumRepository.AddAsync(subForum);
        Console.WriteLine($"SubForum created successfully with ID: {created.Id}");
    }

    private async Task ListSubForumsAsync()
    {
        Console.Clear();
        Console.WriteLine("=== All SubForums ===");

        var subForums = subForumRepository.GetManyAsync().ToList();
        if (!subForums.Any())
        {
            Console.WriteLine("No subforums found.");
            return;
        }

        Console.WriteLine($"{"ID",-5} {"Name",-20} {"Description",-30} {"Creator ID",-12}");
        Console.WriteLine(new string('-', 70));
        foreach (var subForum in subForums)
        {
            var nameDisplay = subForum.Name.Length > 17 ? subForum.Name.Substring(0, 17) + "..." : subForum.Name;
            var descDisplay = subForum.Description.Length > 27 ? subForum.Description.Substring(0, 27) + "..." : subForum.Description;
            Console.WriteLine($"{subForum.Id,-5} {nameDisplay,-20} {descDisplay,-30} {subForum.CreatorUserId,-12}");
        }
    }

    private async Task ViewSubForumAsync()
    {
        Console.Clear();
        Console.WriteLine("=== View SubForum ===");

        Console.Write("Enter SubForum ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        try
        {
            var subForum = await subForumRepository.GetSingleAsync(id);
            var creator = await userRepository.GetSingleAsync(subForum.CreatorUserId);

            Console.WriteLine($"SubForum ID: {subForum.Id}");
            Console.WriteLine($"Name: {subForum.Name}");
            Console.WriteLine($"Description: {subForum.Description}");
            Console.WriteLine($"Creator: {creator.Username} (ID: {creator.Id})");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}