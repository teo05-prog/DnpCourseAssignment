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

        Console.Write("Vote type (1 for upvote, 0 for downvote): ");
        if (!int.TryParse(Console.ReadLine(), out int voteType) ||
            (voteType != 0 && voteType != 1))
        {
            Console.WriteLine(
                "Invalid vote type. Use 1 for upvote, 0 for downvote.");
            return;
        }

        try
        {
            // Validate post and user exist
            await postRepository.GetSingleAsync(postId);
            await userRepository.GetSingleAsync(userId);

            // Check if user has already voted on this post
            var existingVote =
                await postVoteRepository!.GetVoteByUserAndPostAsync(userId,
                    postId);

            if (existingVote != null)
            {
                // Update existing vote
                existingVote.IsUpvote = voteType == 1;
                await postVoteRepository.UpdateAsync(existingVote);
                Console.WriteLine(
                    $"Vote updated to {(voteType == 1 ? "upvote" : "downvote")}.");
            }
            else
            {
                // Create new vote
                var vote = new PostVote
                {
                    PostId = postId,
                    UserId = userId,
                    IsUpvote = voteType == 1
                };

                await postVoteRepository.AddAsync(vote);
                Console.WriteLine(
                    $"Vote cast: {(voteType == 1 ? "upvote" : "downvote")}.");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
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

        Console.Write("Vote type (1 for upvote, 0 for downvote): ");
        if (!int.TryParse(Console.ReadLine(), out int voteType) ||
            (voteType != 0 && voteType != 1))
        {
            Console.WriteLine(
                "Invalid vote type. Use 1 for upvote, 0 for downvote.");
            return;
        }

        try
        {
            // Validate comment and user exist
            await commentRepository.GetSingleAsync(commentId);
            await userRepository.GetSingleAsync(userId);

            // Check if user has already voted on this comment
            var existingVote =
                await commentVoteRepository!.GetVoteByUserAndCommentAsync(
                    userId, commentId);

            if (existingVote != null)
            {
                // Update existing vote
                existingVote.IsUpvote = voteType == 1;
                await commentVoteRepository.UpdateAsync(existingVote);
                Console.WriteLine(
                    $"Vote updated to {(voteType == 1 ? "upvote" : "downvote")}.");
            }
            else
            {
                // Create new vote
                var vote = new CommentVote
                {
                    CommentId = commentId,
                    UserId = userId,
                    IsUpvote = voteType == 1
                };

                await commentVoteRepository.AddAsync(vote);
                Console.WriteLine(
                    $"Vote cast: {(voteType == 1 ? "upvote" : "downvote")}.");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task ViewPostVotesAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Post Votes ===");

        Console.Write("Enter Post ID (or press Enter to see all): ");
        var input = Console.ReadLine()?.Trim();

        try
        {
            if (string.IsNullOrEmpty(input))
            {
                // Show all votes
                var allVotes = postVoteRepository!.GetManyAsync().ToList();
                if (!allVotes.Any())
                {
                    Console.WriteLine("No votes found.");
                    return;
                }

                Console.WriteLine(
                    $"{"Vote ID",-8} {"Post ID",-8} {"User ID",-8} {"Vote Type",-12}");
                Console.WriteLine(new string('-', 40));
                foreach (var vote in allVotes)
                {
                    Console.WriteLine(
                        $"{vote.Id,-8} {vote.PostId,-8} {vote.UserId,-8} {(vote.IsUpvote ? "Upvote" : "Downvote"),-12}");
                }
            }
            else
            {
                if (!int.TryParse(input, out int postId))
                {
                    Console.WriteLine("Invalid Post ID format.");
                    return;
                }

                var post = await postRepository.GetSingleAsync(postId);
                var votes = postVoteRepository!.GetManyAsync()
                    .Where(v => v.PostId == postId).ToList();

                Console.WriteLine($"Votes for post '{post.Title}':");
                if (!votes.Any())
                {
                    Console.WriteLine("No votes found for this post.");
                    return;
                }

                var upvotes = votes.Count(v => v.IsUpvote);
                var downvotes = votes.Count(v => !v.IsUpvote);

                Console.WriteLine($"Upvotes: {upvotes}");
                Console.WriteLine($"Downvotes: {downvotes}");
                Console.WriteLine($"Total Score: {upvotes - downvotes}");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task ViewCommentVotesAsync()
    {
        Console.Clear();
        Console.WriteLine("=== Comment Votes ===");

        Console.Write("Enter Comment ID (or press Enter to see all): ");
        var input = Console.ReadLine()?.Trim();

        try
        {
            if (string.IsNullOrEmpty(input))
            {
                // Show all votes
                var allVotes = commentVoteRepository!.GetManyAsync().ToList();
                if (!allVotes.Any())
                {
                    Console.WriteLine("No votes found.");
                    return;
                }

                Console.WriteLine(
                    $"{"Vote ID",-8} {"Comment ID",-11} {"User ID",-8} {"Vote Type",-12}");
                Console.WriteLine(new string('-', 43));
                foreach (var vote in allVotes)
                {
                    Console.WriteLine(
                        $"{vote.Id,-8} {vote.CommentId,-11} {vote.UserId,-8} {(vote.IsUpvote ? "Upvote" : "Downvote"),-12}");
                }
            }
            else
            {
                if (!int.TryParse(input, out int commentId))
                {
                    Console.WriteLine("Invalid Comment ID format.");
                    return;
                }

                var comment = await commentRepository.GetSingleAsync(commentId);
                var votes = commentVoteRepository!.GetManyAsync()
                    .Where(v => v.CommentId == commentId).ToList();

                Console.WriteLine(
                    $"Votes for comment: '{comment.Body.Substring(0, Math.Min(50, comment.Body.Length))}...'");
                if (!votes.Any())
                {
                    Console.WriteLine("No votes found for this comment.");
                    return;
                }

                var upvotes = votes.Count(v => v.IsUpvote);
                var downvotes = votes.Count(v => !v.IsUpvote);

                Console.WriteLine($"Upvotes: {upvotes}");
                Console.WriteLine($"Downvotes: {downvotes}");
                Console.WriteLine($"Total Score: {upvotes - downvotes}");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}