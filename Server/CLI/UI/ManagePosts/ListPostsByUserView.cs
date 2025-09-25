using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsByUserView
{
    private readonly IPostRepository postRepository;

    public ListPostsByUserView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public void Show(int userId)
    {
        var posts = postRepository.GetManyAsync()
            .Where(p => p.UserId == userId)
            .ToList();

        Console.WriteLine($"Posts by User ID {userId}:");
        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
            return;
        }

        foreach (var post in posts)
        {
            Console.WriteLine(
                $"ID: {post.Id} | Title: {post.Title} | Body: {post.Body}");
        }
    }
}