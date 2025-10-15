using ApiContracts.Comment;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;

    public CommentsController(ICommentRepository commentRepo,
        IUserRepository userRepo, IPostRepository postRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
        this.postRepo = postRepo;
    }

    // POST /comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment(
        [FromBody] CreateCommentDto request)
    {
        try
        {
            // Verify user exists
            var user = await userRepo.GetSingleAsync(request.UserId);
            if (user == null)
                return BadRequest($"User with id {request.UserId} not found");

            // Verify post exists
            var post = await postRepo.GetSingleAsync(request.PostId);
            if (post == null)
                return BadRequest($"Post with id {request.PostId} not found");

            Comment comment = new(request.Body, request.UserId, request.PostId)
            {
                Body = null
            };
            Comment created = await commentRepo.AddAsync(comment);

            CommentDto dto = new()
            {
                Id = created.Id,
                Body = created.Body,
                UserId = created.UserId,
                UserName = user.UserName,
                PostId = created.PostId
            };

            return Created($"/comments/{dto.Id}", dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    // GET /comments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetMany(
        [FromQuery] int? postId = null,
        [FromQuery] int? userId = null,
        [FromQuery] string? userName = null)
    {
        var query = commentRepo.GetManyAsync();
        if (postId.HasValue)
        {
            query = query.Where(c => c.PostId == postId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(c => c.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            // Get user ids matching the userName
            var users = userRepo.GetManyAsync();
            var matchingUserIds = users
                .Where(u =>
                    u.UserName.Contains(userName,
                        StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToHashSet();

            query = query.Where(c => matchingUserIds.Contains(c.UserId));
        }

        var comments = query.ToList();

        var dtos = comments.Select(c =>
        {
            var user = userRepo.GetSingleAsync(c.UserId).Result;
            return new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                UserId = c.UserId,
                UserName = user?.UserName ?? string.Empty,
                PostId = c.PostId
            };
        });
        await Task.CompletedTask;
        return Ok(dtos);
    }

    // GET /comments/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetById(int id)
    {
        var comment = await commentRepo.GetSingleAsync(id);
        if (comment == null)
            return NotFound($"Comment with id {id} not found");
        var user = await userRepo.GetSingleAsync(comment.UserId);
        var dto = new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            UserId = comment.UserId,
            UserName = user?.UserName ?? string.Empty,
            PostId = comment.PostId
        };
        return Ok(dto);
    }

    // DELETE /comments/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        try
        {
            await commentRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Comment with id {id} not found");
        }
    }
}