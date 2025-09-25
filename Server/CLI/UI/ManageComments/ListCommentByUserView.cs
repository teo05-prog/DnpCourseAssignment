using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ListCommentByUserView
{
    private readonly ICommentRepository commentRepository;

    public ListCommentByUserView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public void Show(int userId)
    {
        var comments = commentRepository.GetManyAsync()
            .Where(c => c.UserId == userId)
            .ToList();

        Console.WriteLine($"Comments by User ID {userId}:");
        if (!comments.Any())
        {
            Console.WriteLine("No comments found.");
            return;
        }

        foreach (var comment in comments)
        {
            Console.WriteLine(
                $"ID: {comment.Id} | Post: {comment.PostId} | Body: {comment.Body}");
        }
    }
}