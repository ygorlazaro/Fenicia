using AutoMapper;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;

namespace Fenicia.Auth.Services;

public class UserService(
    IMapper mapper,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    ICompanyRepository companyRepository,
    ISecurityService securityService) : IUserService
{
    public async Task<UserResponse> GetForLoginAsync(TokenRequest request)
    {
        var user = await userRepository.GetByEmailAndCnpjAsync(request.Email, request.Cnpj);

        if (user is null)
        {
            throw new InvalidDataException(TextConstants.InvalidUsernameOrPassword);
        }

        var isValidPassword = securityService.VerifyPassword(request.Password, user.Password);

        if (!isValidPassword)
        {
            throw new InvalidDataException(TextConstants.InvalidUsernameOrPassword);
        }

        return mapper.Map<UserResponse>(user);
    }

    public bool ValidatePasswordAsync(string password, string hashedPassword)
    {
        return securityService.VerifyPassword(password, hashedPassword);
    }

    public async Task<UserResponse?> CreateNewUserAsync(UserRequest request)
    {
        var isExistingUser = await userRepository.CheckUserExistsAsync(request.Email);
        var isExistingCompany = await companyRepository.CheckCompanyExistsAsync(request.Company.Cnpj);

        if (isExistingUser)
        {
            throw new InvalidDataException(TextConstants.EmailExists);
        }

        if (isExistingCompany)
        {
            throw new InvalidDataException(TextConstants.CompanyExists);
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
            Cnpj = request.Company.Cnpj,
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

        var response = mapper.Map<UserResponse>(user);

        return response;
    }

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId)
    {
        return await userRoleRepository.ExistsInCompanyAsync(userId, companyId);
    }
}