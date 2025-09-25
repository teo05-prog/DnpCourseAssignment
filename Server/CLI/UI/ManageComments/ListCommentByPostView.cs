using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ListCommentByPostView
{
    private readonly ICommentRepository commentRepository;

    public ListCommentByPostView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public void Show(int postId)
    {
        var comments = commentRepository.GetManyAsync()
            .Where(c => c.PostId == postId)
            .ToList();

        Console.WriteLine($"Comments for Post ID {postId}:");
        if (!comments.Any())
        {
            Console.WriteLine("No comments found.");
            return;
        }

        foreach (var comment in comments)
        {
            Console.WriteLine(
                $"ID: {comment.Id} | User: {comment.UserId} | Body: {comment.Body}");
        }
    }
}