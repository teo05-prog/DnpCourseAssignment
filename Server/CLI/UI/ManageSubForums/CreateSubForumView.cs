using RepositoryContracts;

namespace CLI.UI.ManageSubForums;

public class CreateSubForumView
{
    private readonly ISubForumRepository subForumRepository;

    public CreateSubForumView(ISubForumRepository subForumRepository)
    {
        this.subForumRepository = subForumRepository;
    }

    public async Task ShowAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Create New SubForum ===");

        Console.Write("Enter SubForum Name: ");
        var name = Console.ReadLine()?.Trim();

        Console.Write("Enter SubForum Description: ");
        var description = Console.ReadLine()?.Trim();

        Console.Write("Enter Creator User ID: ");
        if (!int.TryParse(Console.ReadLine(), out int creatorUserId))
        {
            Console.WriteLine("Invalid User ID.");
            return;
        }

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Name and Description cannot be empty.");
            return;
        }

        var newSubForum = new Entities.SubForum
        {
            Name = name,
            Description = description,
            CreatorUserId = creatorUserId
        };

        await subForumRepository.AddAsync(newSubForum);
        Console.WriteLine($"SubForum '{name}' created successfully.");
    }
}