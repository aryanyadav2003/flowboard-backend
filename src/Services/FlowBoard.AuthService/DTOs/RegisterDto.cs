using System.ComponentModel.DataAnnotations;

namespace FlowBoard.AuthService.DTOs;

public class RegisterDto
{
    [Required, MaxLength(100)]  public string FullName  { get; set; } = "";
    [Required, EmailAddress]    public string Email     { get; set; } = "";
    [Required, MinLength(3)]    public string Username  { get; set; } = "";
    [Required, MinLength(8)]    public string Password  { get; set; } = "";
    public string Role { get; set; } = "Member";
}

public class LoginDto
{
    [Required, EmailAddress]  public string Email    { get; set; } = "";
    [Required]                public string Password { get; set; } = "";
}

public class AuthResponseDto
{
    public string   Token     { get; set; } = "";
    public UserDto  User      { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
}

public class UserDto
{
    public int     UserId    { get; set; }
    public string  FullName  { get; set; } = "";
    public string  Email     { get; set; } = "";
    public string  Username  { get; set; } = "";
    public string  Role      { get; set; } = "";
    public string? AvatarUrl { get; set; }
    public bool    IsActive  { get; set; }
}

public class UpdateProfileDto
{
    public string? FullName  { get; set; }
    public string? Username  { get; set; }
    public string? AvatarUrl { get; set; }
}

public class ChangePasswordDto
{
    [Required] public string CurrentPassword { get; set; } = "";
    [Required, MinLength(8)] public string NewPassword { get; set; } = "";
}