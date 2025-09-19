using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ListCommentView
{
    private readonly ICommentRepository commentRepository;

    public ListCommentView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public async Task ShowAsync()
    {
        Console.Clear();
        Console.WriteLine("=== All Comments ===");

        var comments = commentRepository.GetManyAsync().ToList();
        if (!comments.Any())
        {
            Console.WriteLine("No comments found.");
            return;
        }

        Console.WriteLine(
            $"{"ID",-5} {"Body",-30} {"Post ID",-8} {"User ID",-8}");
        Console.WriteLine(new string('-', 55));
        foreach (var comment in comments)
        {
            var bodyDisplay = comment.Body.Length > 27
                ? comment.Body.Substring(0, 27) + "..."
                : comment.Body;
            Console.WriteLine(
                $"{comment.Id,-5} {bodyDisplay,-30} {comment.PostId,-8} {comment.UserId,-8}");
        }
    }
}