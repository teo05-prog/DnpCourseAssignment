using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class UpdatePostView
{
    private readonly IPostRepository postRepository;

    public UpdatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task ShowAsync(int postId)
    {
        var post = await postRepository.GetSingleAsync(postId);
        if (post == null)
        {
            Console.WriteLine("Post not found.");
            return;
        }

        Console.WriteLine($"Current Title: {post.Title}");
        Console.Write("New Title: ");
        var newTitle = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(newTitle))
        {
            Console.WriteLine("Title cannot be empty.");
            return;
        }

        Console.Write("New Body: ");
        var newBody = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(newBody))
        {
            Console.WriteLine("Body cannot be empty.");
            return;
        }

        post.Title = newTitle;
        post.Body = newBody;
        await postRepository.UpdateAsync(post);
        Console.WriteLine("Post updated successfully.");
    }
}