using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostVoteInMemoryRepository : IPostVoteRepository
{
    private List<PostVote> votes = new List<PostVote>();

    public Task<PostVote> AddAsync(PostVote vote)
    {
        vote.Id = votes.Any() ? votes.Max(v => v.Id) + 1 : 1;
        votes.Add(vote);
        return Task.FromResult(vote);
    }

    public Task UpdateAsync(PostVote vote)
    {
        PostVote? existingVote = votes.SingleOrDefault(v => v.Id == vote.Id);
        if (existingVote is null)
        {
            throw new InvalidOperationException(
                $"Vote with ID '{vote.Id}' not found");
        }

        votes.Remove(existingVote);
        votes.Add(vote);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        PostVote? voteToRemove = votes.SingleOrDefault(v => v.Id == id);
        if (voteToRemove is null)
        {
            throw new InvalidOperationException(
                $"Vote with ID '{id}' not found");
        }

        votes.Remove(voteToRemove);
        return Task.CompletedTask;
    }

    public Task<PostVote> GetSingleAsync(int id)
    {
        PostVote? vote = votes.SingleOrDefault(v => v.Id == id);
        if (vote is null)
        {
            throw new InvalidOperationException(
                $"Vote with ID '{id}' not found");
        }

        return Task.FromResult(vote);
    }

    public IQueryable<PostVote> GetManyAsync()
    {
        return votes.AsQueryable();
    }

    public Task<PostVote?> GetVoteByUserAndPostAsync(int userId, int postId)
    {
        PostVote? vote =
            votes.SingleOrDefault(v =>
                v.UserId == userId && v.PostId == postId);
        return Task.FromResult(vote);
    }
}