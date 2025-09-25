using RepositoryContracts;

namespace CLI.UI.ManageVotes;

public class PostVotesView
{
    private readonly IPostVoteRepository postVoteRepository;

    public PostVotesView(IPostVoteRepository postVoteRepository)
    {
        this.postVoteRepository = postVoteRepository;
    }

    public async Task ShowAsync(int userId, int postId)
    {
        var vote =
            await postVoteRepository.GetVoteByUserAndPostAsync(userId, postId);
        Console.WriteLine($"Votes for Post {postId}:");
        if (vote != null)
        {
            Console.WriteLine(
                $"Vote ID: {vote.Id}, User ID: {vote.UserId}, Post ID: {vote.PostId}, Is Upvote: {vote.IsUpvote}");
        }
        else
        {
            Console.WriteLine(
                "No votes found for this post by the specified user.");
        }
    }
}