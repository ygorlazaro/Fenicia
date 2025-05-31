using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, ICompanyRepository companyRepository, ISecurityService securityService) : IUserService
{
    public async Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj)
    {
        return await userRepository.GetByEmailAndCnpjAsync(email, cnpj);
    }

    public bool ValidatePasswordAsync(string password, string hashedPassword)
    {
        return securityService.VerifyPassword(password, hashedPassword);
    }

    public async Task<UserModel?> CreateNewUserAsync(NewUserRequest request)
    {
        var isExistingUser = await userRepository.CheckUserExistsAsync(request.Email);
        var isExistingCompany = await companyRepository.CheckCompanyExistsAsync(request.Company.CNPJ);

        if (isExistingUser)
        {
            Console.WriteLine("Esse e-mail já existe");

            return null;
        }

        if (isExistingCompany)
        {
            Console.WriteLine("Esse company já existe");

            return null;
        }

        var userRequest = new UserModel
        {
            Email = request.Email,
            Password = securityService.HashPassword(request.Password),
            Name = request.Name
        };

        var user = userRepository.Add(userRequest);
        var company = companyRepository.Add(new CompanyModel
        {
            Name = request.Company.Name,
            Cnpj = request.Company.CNPJ,
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

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId)
    {
        return await userRoleRepository.ExistsInCompanyAsync(userId, companyId);
    }
}