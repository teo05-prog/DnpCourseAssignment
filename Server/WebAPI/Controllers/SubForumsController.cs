using ApiContracts.Post;
using ApiContracts.SubForum;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SubForumsController : ControllerBase
{
    private readonly ISubForumRepository subForumRepo;
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public SubForumsController(
        ISubForumRepository subForumRepo,
        IPostRepository postRepo,
        IUserRepository userRepo)
    {
        this.subForumRepo = subForumRepo;
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    // POST /subforums
    [HttpPost]
    public async Task<ActionResult<SubForumDto>> CreateSubForum([FromBody] CreateSubForumDto request)
    {
        try
        {
            await VerifyNameIsAvailableAsync(request.Name);

            SubForum subForum = new(request.Name, request.Description);
            SubForum created = await subForumRepo.AddAsync(subForum);

            SubForumDto dto = new()
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description
            };

            return Created($"/subforums/{dto.Id}", dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // GET /subforums
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubForumDto>>> GetMany(
        [FromQuery] string? nameContains = null)
    {
        var query = subForumRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(nameContains))
        {
            query = query.Where(sf => 
                sf.Name.Contains(nameContains, StringComparison.OrdinalIgnoreCase));
        }

        var subForums = query.ToList();

        var dtos = subForums.Select(sf => new SubForumDto
        {
            Id = sf.Id,
            Name = sf.Name,
            Description = sf.Description
        });

        await Task.CompletedTask;
        return Ok(dtos);
    }

    // GET /subforums/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubForumDto>> GetById(int id)
    {
        var subForum = await subForumRepo.GetSingleAsync(id);
        if (subForum == null)
            return NotFound($"SubForum with id {id} not found");

        return Ok(new SubForumDto
        {
            Id = subForum.Id,
            Name = subForum.Name,
            Description = subForum.Description
        });
    }

    // GET /subforums/{id}/posts
    [HttpGet("{id:int}/posts")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetSubForumPosts(
        int id,
        [FromQuery] string? titleContains = null)
    {
        var subForum = await subForumRepo.GetSingleAsync(id);
        if (subForum == null)
            return NotFound($"SubForum with id {id} not found");

        var query = postRepo.GetManyAsync()
            .Where(p => p.SubForumId == id);

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            query = query.Where(p => 
                p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));
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
                UserName = user?.UserName ?? "Unknown",
                SubForumId = p.SubForumId
            };
        });

        await Task.CompletedTask;
        return Ok(dtos);
    }

    // PUT /subforums/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSubForum(int id, [FromBody] UpdateSubForumDto dto)
    {
        try
        {
            var existing = await subForumRepo.GetSingleAsync(id);
            if (existing == null)
                return NotFound($"SubForum with id {id} not found");

            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != existing.Name)
            {
                await VerifyNameIsAvailableAsync(dto.Name);
                existing.Name = dto.Name;
            }

            existing.Description = dto.Description ?? existing.Description;

            await subForumRepo.UpdateAsync(existing);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // DELETE /subforums/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await subForumRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"SubForum with id {id} not found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // --- Private helper ---
    private async Task VerifyNameIsAvailableAsync(string name)
    {
        var existing = subForumRepo.GetManyAsync()
            .FirstOrDefault(sf => 
                sf.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
            throw new Exception("SubForum name already in use");

        await Task.CompletedTask;
    }
}