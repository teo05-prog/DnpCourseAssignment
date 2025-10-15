using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class SinglePostView
{
    private readonly IPostRepository postRepository;
    
    public SinglePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }
    
    public async Task ShowAsync(IUserRepository userRepository, int postId)
    {
        Console.Clear();
        Console.WriteLine("=== View Post ===");

        try
        {
            var post = await postRepository.GetSingleAsync(postId);
            var author = await userRepository.GetSingleAsync(post.UserId);

            Console.WriteLine($"Post ID: {post.Id}");
            Console.WriteLine($"Title: {post.Title}");
            Console.WriteLine($"Author: {author.UserName} (ID: {author.Id})");
            Console.WriteLine($"Body:\n{post.Body}");
            Console.WriteLine();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}