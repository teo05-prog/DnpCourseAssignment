namespace ApiContracts.PostVote;

public class UpdatePostVoteDto
{
    public required int UserId { get; set; }
    public bool? IsUpvote { get; set; }
}