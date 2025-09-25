using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageVotes;

public class VotePostView
{
    private readonly IPostVoteRepository postVoteRepository;

    public VotePostView(IPostVoteRepository postVoteRepository)
    {
        this.postVoteRepository = postVoteRepository;
    }

    public async Task VoteAsync(int userId, int postId, bool isUpvote)
    {
        var vote = new PostVote
        {
            UserId = userId,
            PostId = postId,
            IsUpvote = isUpvote
        };
        await postVoteRepository.AddAsync(vote);
        Console.WriteLine("Vote registered for post.");
    }
}