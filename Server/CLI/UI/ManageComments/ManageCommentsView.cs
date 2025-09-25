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

        var createCommentView = new CreateCommentView(commentRepository);
        await createCommentView.ShowAsync(postRepository, userRepository);
    }

    private async Task ListCommentsAsync()
    {
        var listCommentView = new ListCommentView(commentRepository);
        await listCommentView.ShowAsync();
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

        var singleCommentView = new SingleCommentView(commentRepository);
        await singleCommentView.ShowAsync(postRepository, userRepository, id);
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

        var updateCommentView = new UpdateCommentView(commentRepository);
        await updateCommentView.ShowAsync(id);
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

        var deleteCommentView = new DeleteCommentView(commentRepository);
        await deleteCommentView.ShowAsync(id);
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

        var listCommentByUserView =
            new ListCommentByUserView(commentRepository);
        listCommentByUserView.Show(userId);
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

        var listCommentByPostView =
            new ListCommentByPostView(commentRepository);
        listCommentByPostView.Show(postId);
    }
}