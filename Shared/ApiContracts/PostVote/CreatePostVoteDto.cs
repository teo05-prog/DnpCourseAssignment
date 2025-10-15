namespace ApiContracts.PostVote;

public class CreatePostVoteDto
{
    public required int PostId { get; set; }
    public required bool? IsUpvote { get; set; }
    public int UserId { get; set; }
}