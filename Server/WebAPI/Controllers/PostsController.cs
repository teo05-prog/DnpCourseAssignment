using ApiContracts.Post;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    // POST /posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost(
        [FromBody] CreatePostDto request)
    {
        try
        {
            // Optional: Verify user exists
            var user = await userRepo.GetSingleAsync(request.UserId);
            if (user == null)
                return BadRequest($"User with id {request.UserId} not found");

            Post post = new(request.Title, request.Body, request.UserId);
            Post created = await postRepo.AddAsync(post);

            PostDto dto = new()
            {
                Id = created.Id,
                Title = created.Title,
                Body = created.Body,
                UserId = created.UserId,
                UserName = user.UserName
            };

            return Created($"/posts/{dto.Id}", dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // GET /posts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetMany(
        [FromQuery] string? titleContains = null,
        [FromQuery] int? userId = null,
        [FromQuery] string? userName = null)
    {
        var query = postRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            query = query.Where(p =>
                p.Title.Contains(titleContains,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            var users = userRepo.GetManyAsync()
                .Where(u =>
                    u.UserName.Contains(userName,
                        StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();

            query = query.Where(p => users.Contains(p.UserId));
        }

        var posts = query.ToList();

        var dtos = posts.Select(p =>
        {
            var user = userRepo.GetManyAsync()
                .FirstOrDefault(u => u.Id == p.UserId);

            return new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserId = p.UserId,
                UserName = user?.UserName ?? "Unknown"
            };
        });

        await Task.CompletedTask;
        return Ok(dtos);
    }

    // GET /posts/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetById(
        int id,
        [FromQuery] bool includeComments = false)
    {
        var post = await postRepo.GetSingleAsync(id);
        if (post == null)
            return NotFound($"Post with id {id} not found");

        var user = await userRepo.GetSingleAsync(post.UserId);

        var dto = new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,
            UserName = user?.UserName ?? "Unknown"
        };

        return Ok(dto);
    }

    // PUT /posts/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePost(int id,
        [FromBody] UpdatePostDto dto)
    {
        var existing = await postRepo.GetSingleAsync(id);
        if (existing == null)
            return NotFound($"Post with id {id} not found");

        existing.Title = dto.Title ?? existing.Title;
        existing.Body = dto.Body ?? existing.Body;

        await postRepo.UpdateAsync(existing);
        return NoContent();
    }

    // DELETE /posts/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await postRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Post with id {id} not found");
        }
    }
}