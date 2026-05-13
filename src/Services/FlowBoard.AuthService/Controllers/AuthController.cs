using FlowBoard.AuthService.DTOs;
using FlowBoard.AuthService.Interfaces;
using FlowBoard.Shared.DTOs;
using FlowBoard.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register"), AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try {
            var result = await _auth.RegisterAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Registered successfully"));
        }
        catch (InvalidOperationException ex) {
            return Conflict(ApiResponse<AuthResponseDto>.Fail(ex.Message));
        }
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try {
            var result = await _auth.LoginAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful"));
        }
        catch (UnauthorizedAccessException ex) {
            return Unauthorized(ApiResponse<AuthResponseDto>.Fail(ex.Message));
        }
    }

    [HttpGet("profile"), Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _auth.GetProfileAsync(User.GetUserId());
        return Ok(ApiResponse<UserDto>.Ok(result));
    }

    [HttpPut("profile"), Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var result = await _auth.UpdateProfileAsync(User.GetUserId(), dto);
        return Ok(ApiResponse<UserDto>.Ok(result, "Profile updated"));
    }

    [HttpPut("password"), Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        await _auth.ChangePasswordAsync(User.GetUserId(), dto);
        return Ok(ApiResponse.OkNoData("Password changed"));
    }

    [HttpGet("search"), Authorize]
    public async Task<IActionResult> SearchUsers([FromQuery] string query)
    {
        var result = await _auth.SearchUsersAsync(query);
        return Ok(ApiResponse<List<UserDto>>.Ok(result));
    }

    [HttpDelete("deactivate"), Authorize]
    public async Task<IActionResult> Deactivate()
    {
        await _auth.DeactivateAccountAsync(User.GetUserId());
        return Ok(ApiResponse.OkNoData("Account deactivated"));
    }

    [HttpGet("users"), Authorize(Roles = "PlatformAdmin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _auth.GetAllUsersAsync();
        return Ok(ApiResponse<List<UserDto>>.Ok(result));
    }
}