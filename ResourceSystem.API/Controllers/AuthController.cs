using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceSystem.API.Data;
using ResourceSystem.API.Models;
using ResourceSystem.API.Services;

namespace ResourceSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuthService _authService;

    public AuthController(AppDbContext db, IAuthService authService)
    {
        _db = db;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _db.Users.AnyAsync(u => u.Username == request.Username, cancellationToken))
            return BadRequest("Username already exists.");
        var user = new User
        {
            Username = request.Username,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = request.Role ?? "User"
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized();
        var token = _authService.GenerateToken(user.Username, user.Role);
        return Ok(new LoginResponse(token));
    }
}

public record RegisterRequest(string Username, string Password, string? Role);
public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token);
