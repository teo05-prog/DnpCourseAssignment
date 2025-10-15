namespace ApiContracts.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public required string? Body { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int PostId { get; set; }
}