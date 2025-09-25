using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;
    
    public CreatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }
    
    public async Task ShowAsync(IUserRepository userRepository)
    {
        Console.Write("Post Title: ");
        var title = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(title))
        {
            Console.WriteLine("Title cannot be empty.");
            return;
        }

        Console.Write("Post Body: ");
        var body = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(body))
        {
            Console.WriteLine("Body cannot be empty.");
            return;
        }

        Console.Write("User ID (author): ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid User ID format.");
            return;
        }
        
        try
        {
            await userRepository.GetSingleAsync(userId);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("User with specified ID does not exist.");
            return;
        }

        var post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        var created = await postRepository.AddAsync(post);
        Console.WriteLine($"Post created successfully with ID: {created.Id}");
    }
}