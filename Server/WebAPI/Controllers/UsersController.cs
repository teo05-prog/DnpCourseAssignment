using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using ApiContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    // POST /users
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser(
        [FromBody] CreateUserDto request)
    {
        try
        {
            await VerifyUserNameIsAvailableAsync(request.UserName);

            User user = new(request.UserName, request.Password);
            User created = await userRepo.AddAsync(user);

            UserDto dto = new()
            {
                Id = created.Id,
                UserName = created.UserName
            };

            return Created($"/users/{dto.Id}", dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    // GET /users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetMany(
        [FromQuery] string? usernameContains = null)
    {
        var query = userRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(usernameContains))
        {
            query = query.Where(u =>
                u.UserName.Contains(usernameContains,
                    StringComparison.OrdinalIgnoreCase));
        }

        var users = query.ToList();

        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName
        });

        await Task.CompletedTask; // keeps method async
        return Ok(dtos);
    }

    // GET /users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await userRepo.GetSingleAsync(id);
        if (user == null)
            return NotFound($"User with id {id} not found");

        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName
        });
    }

    // PUT /users/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id,
        [FromBody] UpdateUserDto dto)
    {
        var existing = await userRepo.GetSingleAsync(id);
        if (existing == null)
            return NotFound($"User with id {id} not found");

        existing.UserName = dto.UserName ?? existing.UserName;
        if (!string.IsNullOrEmpty(dto.Password))
            existing.Password = dto.Password;

        await userRepo.UpdateAsync(existing);
        return NoContent();
    }

    // DELETE /users/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await userRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"User with id {id} not found");
        }
    }

    // --- Private helper ---
    private async Task VerifyUserNameIsAvailableAsync(string userName)
    {
        var existing = userRepo.GetManyAsync()
            .FirstOrDefault(u =>
                u.UserName.Equals(userName,
                    StringComparison.OrdinalIgnoreCase));

        if (existing != null)
            throw new Exception("Username already in use");

        await Task.CompletedTask;
    }
}