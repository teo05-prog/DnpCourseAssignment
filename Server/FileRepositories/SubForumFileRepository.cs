using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class SubForumFileRepository : ISubForumRepository
{
    private readonly string filePath = "subforums.json";

    public SubForumFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<SubForum>> LoadSubForumsAsync()
    {
        string subForumsAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<SubForum>>(subForumsAsJson) ??
               new List<SubForum>();
    }

    private async Task SaveSubForumsAsync(List<SubForum> subForums)
    {
        string subForumsAsJson = JsonSerializer.Serialize(subForums);
        await File.WriteAllTextAsync(filePath, subForumsAsJson);
    }

    public async Task<SubForum> AddAsync(SubForum subForum)
    {
        var subForums = await LoadSubForumsAsync();
        int maxId = subForums.Count > 0 ? subForums.Max(sf => sf.Id) : 1;
        subForum.Id = maxId + 1;
        subForums.Add(subForum);
        await SaveSubForumsAsync(subForums);
        return subForum;
    }

    public async Task UpdateAsync(SubForum subForum)
    {
        var subForums = await LoadSubForumsAsync();
        var index = subForums.FindIndex(sf => sf.Id == subForum.Id);
        if (index != -1)
        {
            subForums[index] = subForum;
            await SaveSubForumsAsync(subForums);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var subForums = await LoadSubForumsAsync();
        subForums.RemoveAll(sf => sf.Id == id);
        await SaveSubForumsAsync(subForums);
    }

    public async Task<SubForum> GetSingleAsync(int id)
    {
        var subForums = await LoadSubForumsAsync();
        return subForums.FirstOrDefault(sf => sf.Id == id);
    }

    public IQueryable<SubForum> GetManyAsync()
    {
        string subForumsAsJson = File.ReadAllTextAsync(filePath).Result;
        List<SubForum>? subForums =
            JsonSerializer.Deserialize<List<SubForum>>(subForumsAsJson);
        return subForums.AsQueryable();
    }
}