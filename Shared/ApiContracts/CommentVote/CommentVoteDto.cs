namespace ApiContracts.CommentVote;

public class CommentVoteDto
{
    public int Id { get; set; }
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public bool? IsUpvote { get; set; }
}