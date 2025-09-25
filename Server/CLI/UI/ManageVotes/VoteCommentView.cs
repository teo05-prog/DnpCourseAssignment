using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageVotes;

public class VoteCommentView
{
    private readonly ICommentVoteRepository commentVoteRepository;

    public VoteCommentView(ICommentVoteRepository commentVoteRepository)
    {
        this.commentVoteRepository = commentVoteRepository;
    }

    public async Task VoteAsync(int userId, int commentId, bool isUpvote)
    {
        var vote = new CommentVote
        {
            UserId = userId,
            CommentId = commentId,
            IsUpvote = isUpvote
        };
        await commentVoteRepository.AddAsync(vote);
        Console.WriteLine("Vote registered for comment.");
    }
}