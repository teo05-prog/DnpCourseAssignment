namespace Entities;

public class Comment
{
    public int Id { get; set; }
    public required string? Body { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public int? ParentCommentId { get; set; }

    public Comment()
    {
        
    }
    
    public Comment(string body, int userId, int postId)
    {
        Body = body;
        UserId = userId;
        PostId = postId;
    }
}