using Bookenstein.Application.Contracts;
using Bookenstein.Application.Interfaces;
using Bookenstein.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bookenstein.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository) => _repository = repository;

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Post([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var isUserExists = await _repository.GetByEmailAsync(request.Email, ct);
        if (isUserExists is not null)
            return Conflict(new { message = "E-mail já cadastrado." });

        var user = new Users
        {
            Name = request.Name,
            Username = request.Username,
            Email = request.Email,
            Role = request.Role
        };

        await _repository.AddAsync(user, ct);
        await _repository.SaveChangesAsync(ct);

        var response = new UserResponse(user.Id, user.Name, user.Username, user.Email, user.Role);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("/{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) 
            return NotFound();

        var response = new UserResponse(user.Id, user.Name, user.Username, user.Email, user.Role);
        return Ok(response);
    }
}