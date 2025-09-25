using RepositoryContracts;

namespace CLI.UI.ManageSubForums;

public class SingleSubForumView
{
    private readonly ISubForumRepository subForumRepository;

    public SingleSubForumView(ISubForumRepository subForumRepository)
    {
        this.subForumRepository = subForumRepository;
    }

    public async Task ShowAsync()
    {
        Console.Clear();
        Console.WriteLine("=== View SubForum ===");
        Console.Write("Enter SubForum ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid SubForum ID.");
            return;
        }

        var subForum = await subForumRepository.GetSingleAsync(id);
        if (subForum == null)
        {
            Console.WriteLine($"SubForum with ID {id} not found.");
            return;
        }

        Console.WriteLine($"\nSubForum Details:");
        Console.WriteLine($"ID: {subForum.Id}");
        Console.WriteLine($"Name: {subForum.Name}");
        Console.WriteLine($"Description: {subForum.Description}");
        Console.WriteLine($"Creator User ID: {subForum.CreatorUserId}");
    }
}