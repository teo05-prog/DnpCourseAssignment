using ApiContracts.CommentVote;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentVotesController : ControllerBase
{
    private readonly ICommentVoteRepository commentVoteRepo;
    private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;

    public CommentVotesController(ICommentVoteRepository commentVoteRepo,
        IUserRepository userRepo, ICommentRepository commentRepo)
    {
        this.commentVoteRepo = commentVoteRepo;
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
    }

    // POST /commentvotes
    [HttpPost]
    public async Task<ActionResult<CommentVoteDto>> CreateVote(
        [FromBody] CreateCommentVoteDto request)
    {
        try
        {
            // Verify user exists
            var user = await userRepo.GetSingleAsync(request.UserId);
            if (user == null)
                return BadRequest($"User with id {request.UserId} not found");
            // Verify comment exists
            var comment = await commentRepo.GetSingleAsync(request.CommentId);
            if (comment == null)
                return BadRequest(
                    $"Comment with id {request.CommentId} not found");
            // Check if user already voted on this comment
            var existingVote = commentVoteRepo.GetManyAsync()
                .FirstOrDefault(v =>
                    v.UserId == request.UserId &&
                    v.CommentId == request.CommentId);
            if (existingVote != null)
                return BadRequest("User has already voted on this comment");
            CommentVote vote = new(request.UserId, request.CommentId,
                request.IsUpvote);
            CommentVote created = await commentVoteRepo.AddAsync(vote);
            CommentVoteDto dto = new()
            {
                Id = created.Id,
                UserId = created.UserId,
                CommentId = created.CommentId,
                IsUpvote = created.IsUpvote
            };
            return Created($"/commentvotes/{dto.Id}", dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // GET /commentvotes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentVoteDto>>> GetMany(
        [FromQuery] int? userId = null,
        [FromQuery] int? commentId = null)
    {
        var query = commentVoteRepo.GetManyAsync();
        if (userId.HasValue)
        {
            query = query.Where(v => v.UserId == userId.Value);
        }

        if (commentId.HasValue)
        {
            query = query.Where(v => v.CommentId == commentId.Value);
        }

        var entities = query.ToList();
        var dtos = entities.Select(v => new CommentVoteDto
        {
            Id = v.Id,
            UserId = v.UserId,
            CommentId = v.CommentId,
            IsUpvote = v.IsUpvote
        });
        await Task.CompletedTask;
        return Ok(dtos);
    }
    
    //GET /commentvotes/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentVoteDto>> GetSingle(int id)
    {
        var entity = await commentVoteRepo.GetSingleAsync(id);
        if (entity == null)
            return NotFound($"CommentVote with id {id} not found");
        
        var dto = new CommentVoteDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            CommentId = entity.CommentId,
            IsUpvote = entity.IsUpvote
        };
        
        return Ok(dto);
    }
    
    //PUT /commentvotes/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentVoteDto>> UpdateVote(int id, [FromBody] UpdateCommentVoteDto request)
    {
        var existingVote = await commentVoteRepo.GetSingleAsync(id);
        if (existingVote == null)
            return NotFound($"CommentVote with id {id} not found");
        
        existingVote.IsUpvote = request.IsUpvote;
        
        await commentVoteRepo.UpdateAsync(existingVote);
        return NoContent();
    }
    
    // DELETE /commentvotes/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteVote(int id)
    {
        try
        {
            await commentVoteRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"CommentVote with id {id} not found");
        }
    }
}