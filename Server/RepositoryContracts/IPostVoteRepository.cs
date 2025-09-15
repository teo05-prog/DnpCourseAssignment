using Entities;

namespace RepositoryContracts;

public interface IPostVoteRepository
{
    Task<PostVote> AddAsync(PostVote vote);
    Task UpdateAsync(PostVote vote);
    Task DeleteAsync(int id);
    Task<PostVote> GetSingleAsync(int id);
    IQueryable<PostVote> GetManyAsync();
    Task<PostVote?> GetVoteByUserAndPostAsync(int userId, int postId);
}