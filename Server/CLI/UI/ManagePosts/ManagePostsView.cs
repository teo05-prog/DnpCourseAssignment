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

        var createPostView = new CreatePostView(postRepository);
        await createPostView.ShowAsync(userRepository);
    }

    private async Task ListPostsAsync()
    {
        var listPostsView = new ListPostsView(postRepository);
        await listPostsView.ShowAsync(userRepository);
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

        var singlePostView = new SinglePostView(postRepository);
        await singlePostView.ShowAsync(userRepository, id);
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

        var updatePostView = new UpdatePostView(postRepository);
        await updatePostView.ShowAsync(id);
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

        var deletePostView = new DeletePostView(postRepository);
        await deletePostView.ShowAsync(id);
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

        var listPostsByUserView = new ListPostsByUserView(postRepository);
        listPostsByUserView.Show(userId);
    }
}