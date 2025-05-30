using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories;
using Fenicia.Auth.Requests;

namespace Fenicia.Auth.Services;

public class UserService(IUserRepository userRepository, IRoleRepository roleRepository, ICompanyRepository companyRepository) : IUserService
{
    public async Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj)
    {
        return await userRepository.GetByEmailAndCnpjAsync(email, cnpj);
    }

    public Task<bool> ValidatePasswordAsync(string password, string hasedPassword)
    {
        return Task.FromResult(password == hasedPassword);
    }

    public async Task<UserModel?> CreateNewUserAsync(NewUserRequest request)
    {
        var isExintingUser = await userRepository.CheckUserExistsAsync(request.Email);
        var isExintingCompany = await companyRepository.CheckCompanyExistsAsync(request.Company.CNPJ);

        if (isExintingUser)
        {
            Console.WriteLine("Esse e-mail já existe");
            
            return null;
        }

        if (isExintingCompany)
        {
            Console.WriteLine("Esse company já existe");
            
            return null;
        }
        
        var userRequest = new UserModel
        {
            Email = request.Email,
            Password = request.Password,
            Name = request.Name
        };
        
        var user = userRepository.Add(userRequest);
        var company = companyRepository.Add(new CompanyModel()
        {
            Name = request.Company.Name,
            CNPJ = request.Company.CNPJ,
        });
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
                Company = company,
                Role = adminRole
            }
        ];
        
        await userRepository.SaveAsync();

        return user;
    }
}