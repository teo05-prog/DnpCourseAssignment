using RepositoryContracts;

namespace CLI.UI.ManageSubForums;

public class ListSubForumsView
{
    private readonly ISubForumRepository subForumRepository;

    public ListSubForumsView(ISubForumRepository subForumRepository)
    {
        this.subForumRepository = subForumRepository;
    }

    public async Task Show()
    {
        Console.Clear();
        Console.WriteLine("=== List of SubForums ===");

        var subForums = subForumRepository.GetManyAsync().ToList();
        if (!subForums.Any())
        {
            Console.WriteLine("No SubForums available.");
            return;
        }

        foreach (var subForum in subForums)
        {
            Console.WriteLine(
                $"- {subForum.Name}: {subForum.Description} (Creator User ID: {subForum.CreatorUserId})");
        }
    }
}