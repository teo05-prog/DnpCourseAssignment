using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task ShowAsync(IUserRepository userRepository)
    {
        Console.Clear();
        Console.WriteLine("=== All Posts ===");

        var posts = postRepository.GetManyAsync().ToList();
        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
            return;
        }

        Console.WriteLine($"{"ID",-5} {"Title",-30} {"Author",-20}");
        Console.WriteLine(new string('-', 60));
        foreach (var post in posts)
        {
            var author = await userRepository.GetSingleAsync(post.UserId);
            var titleDisplay = post.Title.Length > 27
                ? post.Title.Substring(0, 27) + "..."
                : post.Title;
            Console.WriteLine(
                $"{post.Id,-5} {titleDisplay,-30} {author.UserName,-20}");
        }
    }
}