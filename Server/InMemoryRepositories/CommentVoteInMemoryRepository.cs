using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentVoteInMemoryRepository : ICommentVoteRepository
{
    private List<CommentVote> votes = new List<CommentVote>();

    public Task<CommentVote> AddAsync(CommentVote vote)
    {
        vote.Id = votes.Any() ? votes.Max(v => v.Id) + 1 : 1;
        votes.Add(vote);
        return Task.FromResult(vote);
    }

    public Task UpdateAsync(CommentVote vote)
    {
        CommentVote? existingVote = votes.SingleOrDefault(v => v.Id == vote.Id);
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
        CommentVote? voteToRemove = votes.SingleOrDefault(v => v.Id == id);
        if (voteToRemove is null)
        {
            throw new InvalidOperationException(
                $"Vote with ID '{id}' not found");
        }

        votes.Remove(voteToRemove);
        return Task.CompletedTask;
    }

    public Task<CommentVote> GetSingleAsync(int id)
    {
        CommentVote? vote = votes.SingleOrDefault(v => v.Id == id);
        if (vote is null)
        {
            throw new InvalidOperationException(
                $"Vote with ID '{id}' not found");
        }

        return Task.FromResult(vote);
    }

    public IQueryable<CommentVote> GetManyAsync()
    {
        return votes.AsQueryable();
    }

    public Task<CommentVote?> GetVoteByUserAndCommentAsync(int userId,
        int commentId)
    {
        CommentVote? vote = votes.SingleOrDefault(v =>
            v.UserId == userId && v.CommentId == commentId);
        return Task.FromResult(vote);
    }
}