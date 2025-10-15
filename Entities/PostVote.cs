namespace Entities;

public class PostVote
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public bool? IsUpvote { get; set; }
    
    public PostVote(int postId, int userId, bool? isUpvote)
    {
        Id = postId;
    }

    public PostVote()
    {
    }
}