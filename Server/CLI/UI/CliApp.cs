using CLI.UI.ManageComments;
using CLI.UI.ManagePosts;
using CLI.UI.ManageSubForums;
using CLI.UI.ManageUsers;
using CLI.UI.ManageVotes;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly ISubForumRepository? subForumRepository;
    private readonly IPostVoteRepository? postVoteRepository;
    private readonly ICommentVoteRepository? commentVoteRepository;

    public CliApp(IUserRepository userRepository,
        ICommentRepository commentRepository, IPostRepository postRepository,
        ISubForumRepository? subForumRepository = null,
        IPostVoteRepository? postVoteRepository = null,
        ICommentVoteRepository? commentVoteRepository = null)
    {
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.subForumRepository = subForumRepository;
        this.postVoteRepository = postVoteRepository;
        this.commentVoteRepository = commentVoteRepository;
    }

    public async Task StartAsync()
    {
        Console.WriteLine("=== Welcome to the Forum CLI ===");
        Console.WriteLine();

        while (true)
        {
            ShowMainMenu();
            var choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        var userView = new ManageUsersView(userRepository);
                        await userView.ShowAsync();
                        break;
                    case "2":
                        var postView = new ManagePostsView(postRepository,
                            userRepository);
                        await postView.ShowAsync();
                        break;
                    case "3":
                        var commentView = new ManageCommentsView(
                            commentRepository, postRepository, userRepository);
                        await commentView.ShowAsync();
                        break;
                    case "4":
                        if (subForumRepository != null)
                        {
                            var subForumView =
                                new ManageSubForumsView(subForumRepository,
                                    userRepository);
                            await subForumView.ShowAsync();
                        }
                        else
                        {
                            Console.WriteLine(
                                "SubForum feature is not enabled.");
                        }

                        break;
                    case "5":
                        if (postVoteRepository != null)
                        {
                            var voteView = new ManageVotesView(
                                postVoteRepository, commentVoteRepository,
                                postRepository, commentRepository,
                                userRepository);
                            await voteView.ShowAsync();
                        }
                        else
                        {
                            Console.WriteLine("Voting feature is not enabled.");
                        }

                        break;
                    case "0":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private void ShowMainMenu()
    {
        Console.WriteLine("=== Main Menu ===");
        Console.WriteLine("1. Manage Users");
        Console.WriteLine("2. Manage Posts");
        Console.WriteLine("3. Manage Comments");
        if (subForumRepository != null)
            Console.WriteLine("4. Manage SubForums");
        if (postVoteRepository != null)
            Console.WriteLine("5. Manage Votes");
        Console.WriteLine("0. Exit");
        Console.Write("Choose an option: ");
    }
}