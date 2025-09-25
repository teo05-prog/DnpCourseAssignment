using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<User>> LoadUsersAsync()
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        return System.Text.Json.JsonSerializer.Deserialize<List<User>>(
                   usersAsJson) ??
               new List<User>();
    }

    private async Task SaveUsersAsync(List<User> users)
    {
        string usersAsJson = System.Text.Json.JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, usersAsJson);
    }

    public async Task<User> AddAsync(User user)
    {
        var users = await LoadUsersAsync();
        int maxId = users.Count > 0 ? users.Max(u => u.Id) : 1;
        user.Id = maxId + 1;
        users.Add(user);
        await SaveUsersAsync(users);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        var users = await LoadUsersAsync();
        var index = users.FindIndex(u => u.Id == user.Id);
        if (index != -1)
        {
            users[index] = user;
            await SaveUsersAsync(users);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var users = await LoadUsersAsync();
        users.RemoveAll(u => u.Id == id);
        await SaveUsersAsync(users);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        var users = await LoadUsersAsync();
        return users.FirstOrDefault(u => u.Id == id);
    }

    public IQueryable<User> GetManyAsync()
    {
        string usersAsJson = File.ReadAllTextAsync(filePath).Result;
        List<User>? users = JsonSerializer.Deserialize<List<User>>(usersAsJson);
        return users.AsQueryable();
    }
}