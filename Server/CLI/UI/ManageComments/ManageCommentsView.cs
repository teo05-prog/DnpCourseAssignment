using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ManageCommentsView
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public ManageCommentsView(ICommentRepository commentRepository,
        IPostRepository postRepository, IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            ShowCommentMenu();
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    await CreateCommentAsync();
                    break;
                case "2":
                    await ListCommentsAsync();
                    break;
                case "3":
                    await ViewCommentAsync();
                    break;
                case "4":
                    await UpdateCommentAsync();
                    break;
                case "5":
                    await DeleteCommentAsync();
                    break;
                case "6":
                    await ListCommentsByUserAsync();
                    break;
                case "7":
                    await ListCommentsByPostAsync();
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

    private void ShowCommentMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Comment Management ===");
        Console.WriteLine("1. Create Comment");
        Console.WriteLine("2. List All Comments");
        Console.WriteLine("3. View Comment");
        Console.WriteLine("4. Update Comment");
        Console.WriteLine("5. Delete Comment");
        Console.WriteLine("6. List Comments by User");
        Console.WriteLine("7. List Comments by Post");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Choose an option: ");
    }

    private async Task CreateCommentAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Create New Comment ===");

        Console.Write("Comment Body: ");
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

        // Validate post and user exist
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

    private async Task ListCommentsAsync()
    {
        Console.Clear();
        Console.WriteLine("=== All Comments ===");

        var comments = commentRepository.GetManyAsync().ToList();
        if (!comments.Any())
        {
            Console.WriteLine("No comments found.");
            return;
        }

        Console.WriteLine(
            $"{"ID",-5} {"Body",-30} {"Post ID",-8} {"User ID",-8}");
        Console.WriteLine(new string('-', 55));
        foreach (var comment in comments)
        {
            var bodyDisplay = comment.Body.Length > 27
                ? comment.Body.Substring(0, 27) + "..."
                : comment.Body;
            Console.WriteLine(
                $"{comment.Id,-5} {bodyDisplay,-30} {comment.PostId,-8} {comment.UserId,-8}");
        }
    }

    private async Task ViewCommentAsync()
    {
        Console.Clear();
        Console.WriteLine("=== View Comment ===");

        Console.Write("Enter Comment ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        try
        {
            var comment = await commentRepository.GetSingleAsync(id);
            var author = await userRepository.GetSingleAsync(comment.UserId);
            var post = await postRepository.GetSingleAsync(comment.PostId);

            Console.WriteLine($"Comment ID: {comment.Id}");
            Console.WriteLine($"Author: {author.Username} (ID: {author.Id})");
            Console.WriteLine($"Post: {post.Title} (ID: {post.Id})");
            Console.WriteLine($"Body:\n{comment.Body}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task UpdateCommentAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Update Comment ===");

        Console.Write("Enter Comment ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        try
        {
            var comment = await commentRepository.GetSingleAsync(id);
            Console.WriteLine($"Current Body: {comment.Body}");

            Console.Write("New Body (press Enter to keep current): ");
            var newBody = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(newBody))
            {
                comment.Body = newBody;
            }

            await commentRepository.UpdateAsync(comment);
            Console.WriteLine("Comment updated successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task DeleteCommentAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Comment ===");

        Console.Write("Enter Comment ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        try
        {
            var comment = await commentRepository.GetSingleAsync(id);
            Console.WriteLine(
                $"Are you sure you want to delete this comment? (y/n): ");
            Console.WriteLine($"Comment: {comment.Body}");
            var confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "y" || confirmation == "yes")
            {
                await commentRepository.DeleteAsync(id);
                Console.WriteLine("Comment deleted successfully.");
            }
            else
            {
                Console.WriteLine("Delete operation cancelled.");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task ListCommentsByUserAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Comments by User ===");

        Console.Write("Enter User ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid User ID format.");
            return;
        }

        try
        {
            var user = await userRepository.GetSingleAsync(userId);
            Console.WriteLine($"Comments by {user.Username}:");
            Console.WriteLine();

            var userComments = commentRepository.GetManyAsync()
                .Where(c => c.UserId == userId).ToList();
            if (!userComments.Any())
            {
                Console.WriteLine("No comments found for this user.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Body",-40} {"Post ID",-8}");
            Console.WriteLine(new string('-', 57));
            foreach (var comment in userComments)
            {
                var bodyDisplay = comment.Body.Length > 37
                    ? comment.Body.Substring(0, 37) + "..."
                    : comment.Body;
                Console.WriteLine(
                    $"{comment.Id,-5} {bodyDisplay,-40} {comment.PostId,-8}");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task ListCommentsByPostAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Comments by Post ===");

        Console.Write("Enter Post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid Post ID format.");
            return;
        }

        try
        {
            var post = await postRepository.GetSingleAsync(postId);
            Console.WriteLine($"Comments for '{post.Title}':");
            Console.WriteLine();

            var postComments = commentRepository.GetManyAsync()
                .Where(c => c.PostId == postId).ToList();
            if (!postComments.Any())
            {
                Console.WriteLine("No comments found for this post.");
                return;
            }

            foreach (var comment in postComments)
            {
                try
                {
                    var author =
                        await userRepository.GetSingleAsync(comment.UserId);
                    Console.WriteLine(
                        $"[{comment.Id}] {author.Username}: {comment.Body}");
                }
                catch
                {
                    Console.WriteLine(
                        $"[{comment.Id}] Unknown User: {comment.Body}");
                }

                Console.WriteLine();
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}