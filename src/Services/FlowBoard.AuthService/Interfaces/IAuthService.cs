using FlowBoard.AuthService.DTOs;

namespace FlowBoard.AuthService.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto>  RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto>  LoginAsync(LoginDto dto);
    Task<UserDto>          GetProfileAsync(int userId);
    Task<UserDto>          UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task                   ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<List<UserDto>>    SearchUsersAsync(string query);
    Task                   DeactivateAccountAsync(int userId);
    Task<List<UserDto>>    GetAllUsersAsync();
}