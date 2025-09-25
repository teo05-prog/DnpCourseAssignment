using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class DeletePostView
{
    private readonly IPostRepository postRepository;
    
    public DeletePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }
    
    public async Task ShowAsync(int postId)
    {
        await postRepository.DeleteAsync(postId);
        Console.WriteLine($"Post with ID {postId} deleted.");
    }
}