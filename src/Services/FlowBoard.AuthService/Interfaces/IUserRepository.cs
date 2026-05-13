using FlowBoard.AuthService.Entities;

namespace FlowBoard.AuthService.Interfaces;

public interface IUserRepository
{
    Task<User?>       FindByEmailAsync(string email);
    Task<User?>       FindByUsernameAsync(string username);
    Task<User?>       FindByIdAsync(int userId);
    Task<bool>        ExistsByEmailAsync(string email);
    Task<bool>        ExistsByUsernameAsync(string username);
    Task<List<User>>  FindAllByRoleAsync(string role);
    Task<List<User>>  SearchByFullNameAsync(string query);
    Task<User>        CreateAsync(User user);
    Task<User>        UpdateAsync(User user);
    Task              DeleteAsync(int userId);
}