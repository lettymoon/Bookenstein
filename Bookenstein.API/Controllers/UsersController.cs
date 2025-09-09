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
    private readonly IPasswordHasher _hasher;   

    public UsersController(IUserRepository repository, IPasswordHasher hasher)
        => (_repository, _hasher) = (repository, hasher);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken ct)
    {
        var users = await _repository.GetAllAsync(ct);
        var response = users.Select(u => new UserResponse(u.Id, u.Name, u.Username, u.Email, u.Role));
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) 
            return NotFound();

        var response = new UserResponse(user.Id, user.Name, user.Username, user.Email, user.Role);
        return Ok(response);
    }
}