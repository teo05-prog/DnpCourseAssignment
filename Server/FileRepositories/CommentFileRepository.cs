using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<Comment>> LoadCommentsAsync()
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
    }

    private async Task SaveCommentsAsync(List<Comment> comments)
    {
        string commentsAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentsAsJson);
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        var comments = await LoadCommentsAsync();
        int maxId = comments.Count > 0 ? comments.Max(c => c.Id) : 1;
        comment.Id = maxId + 1;
        comments.Add(comment);
        await SaveCommentsAsync(comments);
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        var comments = await LoadCommentsAsync();
        var index = comments.FindIndex(c => c.Id == comment.Id);
        if (index != -1)
        {
            comments[index] = comment;
            await SaveCommentsAsync(comments);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var comments = await LoadCommentsAsync();
        comments.RemoveAll(c => c.Id == id);
        await SaveCommentsAsync(comments);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        var comments = await LoadCommentsAsync();
        return comments.FirstOrDefault(c => c.Id == id);
    }

    public IQueryable<Comment> GetManyAsync()
    {
        string commentsAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Comment> comments =
            JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        return comments.AsQueryable();
    }
}