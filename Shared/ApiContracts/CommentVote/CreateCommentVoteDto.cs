namespace ApiContracts.CommentVote;

public class CreateCommentVoteDto
{
    public required int CommentId { get; set; }
    public required int UserId { get; set; }
    public required bool IsUpvote { get; set; }
}