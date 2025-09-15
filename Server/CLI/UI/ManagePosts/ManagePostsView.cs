using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostsView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public ManagePostsView(IPostRepository postRepository,
        IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            ShowPostMenu();
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    await CreatePostAsync();
                    break;
                case "2":
                    await ListPostsAsync();
                    break;
                case "3":
                    await ViewPostAsync();
                    break;
                case "4":
                    await UpdatePostAsync();
                    break;
                case "5":
                    await DeletePostAsync();
                    break;
                case "6":
                    await ListPostsByUserAsync();
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

    private void ShowPostMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Post Management ===");
        Console.WriteLine("1. Create Post");
        Console.WriteLine("2. List All Posts");
        Console.WriteLine("3. View Post");
        Console.WriteLine("4. Update Post");
        Console.WriteLine("5. Delete Post");
        Console.WriteLine("6. List Posts by User");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Choose an option: ");
    }

    private async Task CreatePostAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Create New Post ===");

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

        var post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        var created = await postRepository.AddAsync(post);
        Console.WriteLine($"Post created successfully with ID: {created.Id}");
    }

    private async Task ListPostsAsync()
    {
        Console.Clear();
        Console.WriteLine("=== All Posts ===");

        var posts = postRepository.GetManyAsync().ToList();
        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
            return;
        }

        Console.WriteLine($"{"ID",-5} {"Title",-30} {"Author ID",-10}");
        Console.WriteLine(new string('-', 50));
        foreach (var post in posts)
        {
            var titleDisplay = post.Title.Length > 27
                ? post.Title.Substring(0, 27) + "..."
                : post.Title;
            Console.WriteLine(
                $"{post.Id,-5} {titleDisplay,-30} {post.UserId,-10}");
        }
    }

    private async Task ViewPostAsync()
    {
        Console.Clear();
        Console.WriteLine("=== View Post ===");

        Console.Write("Enter Post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        try
        {
            var post = await postRepository.GetSingleAsync(id);
            var author = await userRepository.GetSingleAsync(post.UserId);

            Console.WriteLine($"Post ID: {post.Id}");
            Console.WriteLine($"Title: {post.Title}");
            Console.WriteLine($"Author: {author.Username} (ID: {author.Id})");
            Console.WriteLine($"Body:\n{post.Body}");
            Console.WriteLine();

            // Show comments for this post
            await ShowCommentsForPostAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task ShowCommentsForPostAsync(int postId)
    {
        // This would require access to comment repository
        Console.WriteLine("--- Comments ---");
        Console.WriteLine(
            "(Comment viewing requires comment repository access)");
    }

    private async Task UpdatePostAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Update Post ===");

        Console.Write("Enter Post ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        try
        {
            var post = await postRepository.GetSingleAsync(id);
            Console.WriteLine($"Current Title: {post.Title}");
            Console.WriteLine($"Current Body: {post.Body}");

            Console.Write("New Title (press Enter to keep current): ");
            var newTitle = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(newTitle))
            {
                post.Title = newTitle;
            }

            Console.Write("New Body (press Enter to keep current): ");
            var newBody = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(newBody))
            {
                post.Body = newBody;
            }

            await postRepository.UpdateAsync(post);
            Console.WriteLine("Post updated successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task DeletePostAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Post ===");

        Console.Write("Enter Post ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID format.");
            return;
        }

        try
        {
            var post = await postRepository.GetSingleAsync(id);
            Console.WriteLine(
                $"Are you sure you want to delete post '{post.Title}'? (y/n): ");
            var confirmation = Console.ReadLine()?.ToLower();

            if (confirmation == "y" || confirmation == "yes")
            {
                await postRepository.DeleteAsync(id);
                Console.WriteLine("Post deleted successfully.");
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

    private async Task ListPostsByUserAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Posts by User ===");

        Console.Write("Enter User ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid User ID format.");
            return;
        }

        try
        {
            var user = await userRepository.GetSingleAsync(userId);
            Console.WriteLine($"Posts by {user.Username}:");
            Console.WriteLine();

            var userPosts = postRepository.GetManyAsync()
                .Where(p => p.UserId == userId).ToList();
            if (!userPosts.Any())
            {
                Console.WriteLine("No posts found for this user.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Title",-40}");
            Console.WriteLine(new string('-', 50));
            foreach (var post in userPosts)
            {
                var titleDisplay = post.Title.Length > 37
                    ? post.Title.Substring(0, 37) + "..."
                    : post.Title;
                Console.WriteLine($"{post.Id,-5} {titleDisplay,-40}");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}