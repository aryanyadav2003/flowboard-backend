using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FlowBoard.AuthService.DTOs;
using FlowBoard.AuthService.Entities;
using FlowBoard.AuthService.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace FlowBoard.AuthService.Services;

public class AuthServiceImpl : IAuthService
{
    private readonly IUserRepository _repo;
    private readonly IConfiguration  _config;
    private readonly IMapper         _mapper;

    public AuthServiceImpl(IUserRepository repo, IConfiguration config, IMapper mapper)
    {
        _repo   = repo;
        _config = config;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _repo.ExistsByEmailAsync(dto.Email))
            throw new InvalidOperationException("Email already registered.");

        if (await _repo.ExistsByUsernameAsync(dto.Username))
            throw new InvalidOperationException("Username already taken.");

        var user = new User
        {
            FullName     = dto.FullName,
            Email        = dto.Email,
            Username     = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role         = string.IsNullOrWhiteSpace(dto.Role) ? "Member" : dto.Role
        };

        await _repo.CreateAsync(user);
        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _repo.FindByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated.");

        return BuildAuthResponse(user);
    }

    public async Task<UserDto> GetProfileAsync(int userId)
    {
        var user = await _repo.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _repo.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        if (dto.FullName  != null) user.FullName  = dto.FullName;
        if (dto.Username  != null) user.Username  = dto.Username;
        if (dto.AvatarUrl != null) user.AvatarUrl = dto.AvatarUrl;

        await _repo.UpdateAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _repo.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _repo.UpdateAsync(user);
    }

    public async Task<List<UserDto>> SearchUsersAsync(string query)
    {
        var users = await _repo.SearchByFullNameAsync(query);
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task DeactivateAccountAsync(int userId)
    {
        var user = await _repo.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        user.IsActive = false;
        await _repo.UpdateAsync(user);
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _repo.FindAllByRoleAsync("Member");
        return _mapper.Map<List<UserDto>>(users);
    }

    private AuthResponseDto BuildAuthResponse(User user)
    {
        var key      = _config["Jwt:Key"]!;
        var issuer   = _config["Jwt:Issuer"]!;
        var audience = _config["Jwt:Audience"]!;
        var expiry   = DateTime.UtcNow.AddHours(
                           int.Parse(_config["Jwt:ExpiryHours"] ?? "24"));

        var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds  = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email,          user.Email),
            new Claim(ClaimTypes.Name,           user.FullName),
            new Claim(ClaimTypes.Role,           user.Role),
            new Claim("username",                user.Username)
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            expiry,
            signingCredentials: creds);

        return new AuthResponseDto
        {
            Token     = new JwtSecurityTokenHandler().WriteToken(token),
            User      = _mapper.Map<UserDto>(user),
            ExpiresAt = expiry
        };
    }
}