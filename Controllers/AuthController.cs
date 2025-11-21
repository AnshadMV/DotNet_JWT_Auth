using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserRepository _repo;
    private readonly IConfiguration _config;

    public AuthController(UserRepository repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    [HttpPost("register")]
    public IActionResult Register(string username, string password, string role = "User")
    {
        if (_repo.GetByUsername(username) != null)
            return BadRequest("User already exists");

        var hashed = BCrypt.Net.BCrypt.HashPassword(password);

        _repo.AddUser(new AppUser
        {
            Username = username,
            Password = hashed,
            Role = role
        });

        return Ok("User registered");
    }

    [HttpPost("login")]
    public IActionResult Login(string username, string password)
    {
        var user = _repo.GetByUsername(username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return Unauthorized("Invalid credentials");

        var token = GenerateJwt(user);
        return Ok(new { token });
    }
    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        return Ok(_repo.GetAllUsers());
    }

    private string GenerateJwt(AppUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
