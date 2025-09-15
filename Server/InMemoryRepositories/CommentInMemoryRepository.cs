using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private List<Comment> comments = new List<Comment>();

    public CommentInMemoryRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        comments.AddRange(new[]
        {
            new Comment
            {
                Id = 1, Body = "Great post! Thanks for sharing.", PostId = 1,
                UserId = 2
            },
            new Comment
            {
                Id = 2, Body = "I agree with your points about C#.", PostId = 2,
                UserId = 3
            },
            new Comment
            {
                Id = 3, Body = "Very informative discussion!", PostId = 3,
                UserId = 1
            }
        });
    }

    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id =
            comments.Any() ? comments.Max(c => c.Id) + 1 : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment =
            comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove =
            comments.SingleOrDefault(c => c.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetManyAsync()
    {
        return comments.AsQueryable();
    }
}