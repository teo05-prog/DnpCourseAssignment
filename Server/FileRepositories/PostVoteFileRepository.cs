using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostVoteFileRepository : IPostVoteRepository
{
    private readonly string filePath = "postVotes.json";

    public PostVoteFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<PostVote>> LoadVotesAsync()
    {
        string votesAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<PostVote>>(votesAsJson) ??
               new List<PostVote>();
    }

    private async Task SaveVotesAsync(List<PostVote> votes)
    {
        string votesAsJson = JsonSerializer.Serialize(votes);
        await File.WriteAllTextAsync(filePath, votesAsJson);
    }

    public async Task<PostVote> AddAsync(PostVote vote)
    {
        var votes = await LoadVotesAsync();
        int maxId = votes.Count > 0 ? votes.Max(v => v.Id) : 1;
        vote.Id = maxId + 1;
        votes.Add(vote);
        await SaveVotesAsync(votes);
        return vote;
    }

    public async Task UpdateAsync(PostVote vote)
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

    public async Task<PostVote> GetSingleAsync(int id)
    {
        var votes = await LoadVotesAsync();
        return votes.FirstOrDefault(v => v.Id == id)!;
    }

    public IQueryable<PostVote> GetManyAsync()
    {
        string votesAsJson = File.ReadAllTextAsync(filePath).Result;
        List<PostVote> votes =
            JsonSerializer.Deserialize<List<PostVote>>(votesAsJson) ??
            new List<PostVote>();
        return votes.AsQueryable();
    }

    public async Task<PostVote?> GetVoteByUserAndPostAsync(int userId,
        int postId)
    {
        var votes = await LoadVotesAsync();
        return votes.FirstOrDefault(v =>
            v.UserId == userId && v.PostId == postId);
    }
}