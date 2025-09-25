using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageVotes;

public class ManageVotesView
{
    private readonly IPostVoteRepository? postVoteRepository;
    private readonly ICommentVoteRepository? commentVoteRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public ManageVotesView(IPostVoteRepository? postVoteRepository,
        ICommentVoteRepository? commentVoteRepository,
        IPostRepository postRepository, ICommentRepository commentRepository,
        IUserRepository userRepository)
    {
        this.postVoteRepository = postVoteRepository;
        this.commentVoteRepository = commentVoteRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            ShowVoteMenu();
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    if (postVoteRepository != null)
                        await VoteOnPostAsync();
                    else
                        Console.WriteLine("Post voting is not enabled.");
                    break;
                case "2":
                    if (commentVoteRepository != null)
                        await VoteOnCommentAsync();
                    else
                        Console.WriteLine("Comment voting is not enabled.");
                    break;
                case "3":
                    if (postVoteRepository != null)
                        await ViewPostVotesAsync();
                    else
                        Console.WriteLine("Post voting is not enabled.");
                    break;
                case "4":
                    if (commentVoteRepository != null)
                        await ViewCommentVotesAsync();
                    else
                        Console.WriteLine("Comment voting is not enabled.");
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

    private void ShowVoteMenu()
    {
        Console.Clear();
        Console.WriteLine("=== Vote Management ===");
        Console.WriteLine("1. Vote on Post");
        Console.WriteLine("2. Vote on Comment");
        Console.WriteLine("3. View Post Votes");
        Console.WriteLine("4. View Comment Votes");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Choose an option: ");
    }

    private async Task VoteOnPostAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Vote on Post ===");

        Console.Write("Post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid Post ID format.");
            return;
        }

        Console.Write("User ID (voter): ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid User ID format.");
            return;
        }

        Console.Write("Is this an upvote? (y/n): ");
        var upvoteInput = Console.ReadLine()?.Trim().ToLower();
        bool isUpvote = upvoteInput == "y";

        var voteView = new VotePostView(postVoteRepository!);
        await voteView.VoteAsync(userId, postId, isUpvote);
    }

    private async Task VoteOnCommentAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Vote on Comment ===");

        Console.Write("Comment ID: ");
        if (!int.TryParse(Console.ReadLine(), out int commentId))
        {
            Console.WriteLine("Invalid Comment ID format.");
            return;
        }

        Console.Write("User ID (voter): ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid User ID format.");
            return;
        }

        Console.Write("Is this an upvote? (y/n): ");
        var upvoteInput = Console.ReadLine()?.Trim().ToLower();
        bool isUpvote = upvoteInput == "y";

        var voteView = new VoteCommentView(commentVoteRepository!);
        await voteView.VoteAsync(userId, commentId, isUpvote);
    }

    private async Task ViewPostVotesAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Post Votes ===");

        Console.Write("Post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid Post ID format.");
            return;
        }

        Console.Write("User ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid User ID format.");
            return;
        }

        var view = new PostVotesView(postVoteRepository!);
        await view.ShowAsync(userId, postId);
    }

    private async Task ViewCommentVotesAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Comment Votes ===");

        Console.Write("Comment ID: ");
        if (!int.TryParse(Console.ReadLine(), out int commentId))
        {
            Console.WriteLine("Invalid Comment ID format.");
            return;
        }

        Console.Write("User ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid User ID format.");
            return;
        }

        var view = new CommentVotesView(commentVoteRepository!);
        await view.ShowAsync(userId, commentId);
    }
}