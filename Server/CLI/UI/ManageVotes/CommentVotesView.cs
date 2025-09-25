using RepositoryContracts;

namespace CLI.UI.ManageVotes;

public class CommentVotesView
{
    private readonly ICommentVoteRepository commentVoteRepository;

    public CommentVotesView(ICommentVoteRepository commentVoteRepository)
    {
        this.commentVoteRepository = commentVoteRepository;
    }

    public async Task ShowAsync(int userId, int commentId)
    {
        var vote =
            await commentVoteRepository.GetVoteByUserAndCommentAsync(userId,
                commentId);
        Console.WriteLine($"Votes for Comment {commentId}:");
        if (vote != null)
        {
            Console.WriteLine(
                $"Vote ID: {vote.Id}, User ID: {vote.UserId}, Comment ID: {vote.CommentId}, Is Upvote: {vote.IsUpvote}");
        }
        else
        {
            Console.WriteLine(
                "No votes found for this comment by the specified user.");
        }
    }
}