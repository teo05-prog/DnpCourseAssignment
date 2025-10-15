using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class SingleCommentView
{
    private readonly ICommentRepository commentRepository;

    public SingleCommentView(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    public async Task ShowAsync(IPostRepository postRepository,
        IUserRepository userRepository, int commentId)
    {
        Console.Clear();
        Console.WriteLine("=== View Comment ===");

        try
        {
            var comment = await commentRepository.GetSingleAsync(commentId);
            var author = await userRepository.GetSingleAsync(comment.UserId);
            var post = await postRepository.GetSingleAsync(comment.PostId);

            Console.WriteLine($"Comment ID: {comment.Id}");
            Console.WriteLine($"Author: {author.UserName} (ID: {author.Id})");
            Console.WriteLine($"Post: {post.Title} (ID: {post.Id})");
            Console.WriteLine($"Body:\n{comment.Body}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}