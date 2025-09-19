using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class CreateCommentView
{
    private readonly ICommentRepository commentRepository;

    public CreateCommentView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public async Task ShowAsync(IPostRepository postRepository,
        IUserRepository userRepository)
    {
        Console.WriteLine("Comment Body: ");
        var body = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(body))
        {
            Console.WriteLine("Body cannot be empty.");
            return;
        }

        Console.Write("Post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid Post ID format.");
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
            await postRepository.GetSingleAsync(postId);
            await userRepository.GetSingleAsync(userId);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Validation failed: {ex.Message}");
            return;
        }

        var comment = new Comment
        {
            Body = body,
            PostId = postId,
            UserId = userId
        };

        var created = await commentRepository.AddAsync(comment);
        Console.WriteLine(
            $"Comment created successfully with ID: {created.Id}");
    }
}