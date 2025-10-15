namespace Entities;

public class CommentVote
{
    public int Id { get; set; }
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public bool? IsUpvote { get; set; }

    public CommentVote()
    {
        
    }
    
    public CommentVote(int userId, int commentId, bool isUpvote)
    {
        UserId = userId;
        CommentId = commentId;
        IsUpvote = isUpvote;
    }
}