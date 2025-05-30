using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories;
using Fenicia.Auth.Requests;

namespace Fenicia.Auth.Services;

public class UserService(IUserRepository userRepository, IRoleRepository roleRepository) : IUserService
{
    public async Task<UserModel?> GetByEmailAsync(string email)
    {
        return await userRepository.GetByEmailAsync(email);
    }

    public Task<bool> ValidatePasswordAsync(string password, string hasedPassword)
    {
        return Task.FromResult(password == hasedPassword);
    }

    public async Task<UserModel?> CreateNewUserAsync(NewUserRequest request)
    {
        var isExintingUser = await userRepository.CheckUserExistsAsync(request.Email);

        if (isExintingUser)
        {
            Console.WriteLine("Esse e-mail já existe");
            
            return null;
        }
        
        var userRequest = new UserModel
        {
            Email = request.Email,
            Password = request.Password,
            Name = request.Name
        };
        
        var user = userRepository.Add(userRequest);
        var adminRole = await roleRepository.GetAdminRoleAsync();

        if (adminRole is null)
        {
            return null;
        }

        user.UsersRoles =
        [
            new UserRoleModel
            {
                User = user,
                Role = adminRole
            }
        ];
        
        await userRepository.SaveAsync();

        return user;
    }
}