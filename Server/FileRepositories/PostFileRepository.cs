using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<Post>> LoadPostsAsync()
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Post>>(postsAsJson) ??
               new List<Post>();
    }

    private async Task SavePostsAsync(List<Post> posts)
    {
        string postsAsJson = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, postsAsJson);
    }

    public async Task<Post> AddAsync(Post post)
    {
        var posts = await LoadPostsAsync();
        int maxId = posts.Count > 0 ? posts.Max(p => p.Id) : 1;
        post.Id = maxId + 1;
        posts.Add(post);
        await SavePostsAsync(posts);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        var posts = await LoadPostsAsync();
        var index = posts.FindIndex(p => p.Id == post.Id);
        if (index != -1)
        {
            posts[index] = post;
            await SavePostsAsync(posts);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var posts = await LoadPostsAsync();
        posts.RemoveAll(p => p.Id == id);
        await SavePostsAsync(posts);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        var posts = await LoadPostsAsync();
        return posts.FirstOrDefault(p => p.Id == id);
    }

    public IQueryable<Post> GetManyAsync()
    {
        string postsAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Post>? posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson);
        return posts.AsQueryable();
    }
}