namespace ApiContracts.PostVote;

public class PostVoteDto
{
    public required int Id { get; set; }
    public required int PostId { get; set; }
    public required int UserId { get; set; }
    public required bool? IsUpvote { get; set; }
}