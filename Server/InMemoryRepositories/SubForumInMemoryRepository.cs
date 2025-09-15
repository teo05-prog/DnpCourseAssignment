using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class SubForumInMemoryRepository : ISubForumRepository
{
    private List<SubForum> subForums = new List<SubForum>();

    public SubForumInMemoryRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        subForums.AddRange(new[]
        {
            new SubForum
            {
                Id = 1, Name = "General Discussion",
                Description = "General topics and discussions",
                CreatorUserId = 1
            },
            new SubForum
            {
                Id = 2, Name = "Programming",
                Description = "Programming related discussions",
                CreatorUserId = 2
            },
            new SubForum
            {
                Id = 3, Name = "Web Development",
                Description = "Web development topics", CreatorUserId = 3
            }
        });
    }

    public Task<SubForum> AddAsync(SubForum subForum)
    {
        subForum.Id = subForums.Any() ? subForums.Max(sf => sf.Id) + 1 : 1;
        subForums.Add(subForum);
        return Task.FromResult(subForum);
    }

    public Task UpdateAsync(SubForum subForum)
    {
        SubForum? existingSubForum =
            subForums.SingleOrDefault(sf => sf.Id == subForum.Id);
        if (existingSubForum is null)
        {
            throw new InvalidOperationException(
                $"SubForum with ID '{subForum.Id}' not found");
        }

        subForums.Remove(existingSubForum);
        subForums.Add(subForum);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        SubForum? subForumToRemove =
            subForums.SingleOrDefault(sf => sf.Id == id);
        if (subForumToRemove is null)
        {
            throw new InvalidOperationException(
                $"SubForum with ID '{id}' not found");
        }

        subForums.Remove(subForumToRemove);
        return Task.CompletedTask;
    }

    public Task<SubForum> GetSingleAsync(int id)
    {
        SubForum? subForum = subForums.SingleOrDefault(sf => sf.Id == id);
        if (subForum is null)
        {
            throw new InvalidOperationException(
                $"SubForum with ID '{id}' not found");
        }

        return Task.FromResult(subForum);
    }

    public IQueryable<SubForum> GetManyAsync()
    {
        return subForums.AsQueryable();
    }
}