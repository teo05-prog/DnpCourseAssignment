using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class DeleteCommentView
{
    private readonly ICommentRepository commentRepository;

    public DeleteCommentView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public async Task ShowAsync(int commentId)
    {
        await commentRepository.DeleteAsync(commentId);
        Console.WriteLine($"Comment with ID {commentId} deleted.");
    }
}