using Entities;

namespace RepositoryContracts;

public interface ICommentVoteRepository
{
    Task<CommentVote> AddAsync(CommentVote vote);
    Task UpdateAsync(CommentVote vote);
    Task DeleteAsync(int id);
    Task<CommentVote> GetSingleAsync(int id);
    IQueryable<CommentVote> GetManyAsync();
    Task<CommentVote?> GetVoteByUserAndCommentAsync(int userId, int commentId);
}