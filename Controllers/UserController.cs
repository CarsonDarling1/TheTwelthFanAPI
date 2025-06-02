using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheTwelthFan.Data;
using TheTwelthFan.Models;
using TheTwelthFanAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    private readonly TodoContext _context;
    private readonly ITokenService _tokenService;

    public UsersController(TodoContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

 [HttpPost("create")]
public async Task<ActionResult<User>> CreateUser(User user)
{
    try
    {
        // Check if the user exists in the database
        var userCheck = await _context.Users.SingleOrDefaultAsync(u => u.username == user.username);
        if(userCheck != null){
            Console.WriteLine($"Username already exists: {user.username}");
            return Conflict(new { Message = $"Username already exists: {user.username}" });
        }
        // Hash the password before storing it in the database
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password);
        user.password = hashedPassword;

        // Log the user details for debugging
        Console.WriteLine($"Creating user with username: {user.username}");

        // Add the user to the database
        _context.Users.Add(user);
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            // Log success
            Console.WriteLine($"Successfully created user: {user.username}");

            // Return the created user (excluding sensitive info like the password)
            return CreatedAtAction(nameof(GetUsers), new { id = user.id }, new 
            {
                user.id,
                user.username
            });
        }
        else
        {
            // Log if no rows were affected
            Console.WriteLine("No rows were affected during save.");
            return BadRequest("User creation failed.");
        }
    }
    catch (Exception ex)
    {
        // Log the error details
        Console.WriteLine($"Error creating user: {ex.Message}");
        return StatusCode(500, "Internal server error");
    }
}



    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login request)
    {
        // Check if the user exists in the database
        var user = await _context.Users.SingleOrDefaultAsync(u => u.username == request.username);
        if (user == null)
        {
            return Unauthorized(new { Message = "Invalid username or password" });
        }
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);

        // Verify the password (assumes passwords are hashed)
        var passwordIsValid = BCrypt.Net.BCrypt.Verify(request.password, user.password);
        if (!passwordIsValid)
        {
            return Unauthorized(new { Message = "Invalid username or password" });
        }

        // Generate JWT token
        var token = _tokenService.GenerateToken(request.username); // Generate token logic

        return Ok(new 
        { 
            access_token = token  // Use 'access_token' key (Swagger detects this)
        });
    }

    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }

        // Optional: Ensure the authenticated user is allowed to update this record
        var usernameFromToken = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (usernameFromToken != user.username)
        {
            return Forbid(); // User not authorized to update this record
        }

        // Update user fields
        user.email = request.email ?? user.email;

        // Save changes
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User updated successfully" });
    }

    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Ensure the authenticated user is deleting their own account
            var userIdFromToken = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userIdFromToken != id)
            {
                return Forbid();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }


}
