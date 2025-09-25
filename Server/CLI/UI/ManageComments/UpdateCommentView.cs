using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class UpdateCommentView
{
    private readonly ICommentRepository commentRepository;

    public UpdateCommentView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public async Task ShowAsync(int commentId)
    {
        var comment = await commentRepository.GetSingleAsync(commentId);
        if (comment == null)
        {
            Console.WriteLine("Comment not found.");
            return;
        }

        Console.WriteLine($"Current Body: {comment.Body}");
        Console.Write("New Body: ");
        var newBody = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(newBody))
        {
            Console.WriteLine("Body cannot be empty.");
            return;
        }

        comment.Body = newBody;
        await commentRepository.UpdateAsync(comment);
        Console.WriteLine("Comment updated successfully.");
    }
}