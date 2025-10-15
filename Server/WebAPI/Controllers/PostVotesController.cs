using ApiContracts.PostVote;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostVotesController : ControllerBase
{
    private readonly IPostVoteRepository postVoteRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;

    public PostVotesController(IPostVoteRepository postVoteRepo,
        IUserRepository userRepo, IPostRepository postRepo)
    {
        this.postVoteRepo = postVoteRepo;
        this.userRepo = userRepo;
        this.postRepo = postRepo;
    }
    
    // POST /postvotes
    [HttpPost]
    public async Task<ActionResult<PostVoteDto>> CreateVote([FromBody] CreatePostVoteDto request)
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

            // Check if user already voted on this post
            var existingVote = postVoteRepo.GetManyAsync()
                .FirstOrDefault(v => v.UserId == request.UserId && v.PostId == request.PostId);

            if (existingVote != null)
                return BadRequest("User has already voted on this post");

            PostVote vote = new(request.UserId, request.PostId, request.IsUpvote);
            PostVote created = await postVoteRepo.AddAsync(vote);

            PostVoteDto dto = new()
            {
                Id = created.Id,
                UserId = created.UserId,
                PostId = created.PostId,
                IsUpvote = created.IsUpvote
            };

            return Created($"/postvotes/{dto.Id}", dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // GET /postvotes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostVoteDto>>> GetMany(
        [FromQuery] int? userId = null,
        [FromQuery] int? postId = null)
    {
        var query = postVoteRepo.GetManyAsync();

        if (userId.HasValue)
        {
            query = query.Where(v => v.UserId == userId.Value);
        }

        if (postId.HasValue)
        {
            query = query.Where(v => v.PostId == postId.Value);
        }

        var votes = query.ToList();

        var dtos = votes.Select(v => new PostVoteDto
        {
            Id = v.Id,
            UserId = v.UserId,
            PostId = v.PostId,
            IsUpvote = v.IsUpvote
        });

        await Task.CompletedTask;
        return Ok(dtos);
    }

    // GET /postvotes/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostVoteDto>> GetById(int id)
    {
        var vote = await postVoteRepo.GetSingleAsync(id);
        if (vote == null)
            return NotFound($"PostVote with id {id} not found");

        return Ok(new PostVoteDto
        {
            Id = vote.Id,
            UserId = vote.UserId,
            PostId = vote.PostId,
            IsUpvote = vote.IsUpvote
        });
    }

    // PUT /postvotes/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateVote(int id, [FromBody] UpdatePostVoteDto dto)
    {
        var existing = await postVoteRepo.GetSingleAsync(id);
        if (existing == null)
            return NotFound($"PostVote with id {id} not found");

        existing.IsUpvote = dto.IsUpvote;

        await postVoteRepo.UpdateAsync(existing);
        return NoContent();
    }

    // DELETE /postvotes/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await postVoteRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"PostVote with id {id} not found");
        }
    }
}