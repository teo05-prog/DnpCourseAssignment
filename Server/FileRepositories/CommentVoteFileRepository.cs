using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentVoteFileRepository : ICommentVoteRepository
{
    private readonly string filePath = "commentVotes.json";

    public CommentVoteFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<CommentVote>> LoadVotesAsync()
    {
        string votesAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<CommentVote>>(votesAsJson)!;
    }

    private async Task SaveVotesAsync(List<CommentVote> votes)
    {
        string votesAsJson = JsonSerializer.Serialize(votes);
        await File.WriteAllTextAsync(filePath, votesAsJson);
    }

    public async Task<CommentVote> AddAsync(CommentVote vote)
    {
        var votes = await LoadVotesAsync();
        int maxId = votes.Count > 0 ? votes.Max(v => v.Id) : 1;
        vote.Id = maxId + 1;
        votes.Add(vote);
        await SaveVotesAsync(votes);
        return vote;
    }

    public async Task UpdateAsync(CommentVote vote)
    {
        var votes = await LoadVotesAsync();
        var index = votes.FindIndex(v => v.Id == vote.Id);
        if (index != -1)
        {
            votes[index] = vote;
            await SaveVotesAsync(votes);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var votes = await LoadVotesAsync();
        votes.RemoveAll(v => v.Id == id);
        await SaveVotesAsync(votes);
    }

    public async Task<CommentVote> GetSingleAsync(int id)
    {
        var votes = await LoadVotesAsync();
        return votes.FirstOrDefault(v => v.Id == id)!;
    }

    public IQueryable<CommentVote> GetManyAsync()
    {
        string votesAsJson = File.ReadAllTextAsync(filePath).Result;
        List<CommentVote> commentVotes =
            JsonSerializer.Deserialize<List<CommentVote>>(votesAsJson)!;
        return commentVotes.AsQueryable();
    }

    public async Task<CommentVote?> GetVoteByUserAndCommentAsync(int userId,
        int commentId)
    {
        var votes = await LoadVotesAsync();
        return votes.FirstOrDefault(v =>
            v.UserId == userId && v.CommentId == commentId);
    }
}