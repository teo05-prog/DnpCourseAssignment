namespace Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
    public int? SubForumId { get; set; }
    
    public Post ()
    {
        Title = string.Empty;
        Body = string.Empty;
    }
    
    public Post (string title, string body, int userId)
    {
        Title = title;
        Body = body;
        UserId = userId;
    }
}