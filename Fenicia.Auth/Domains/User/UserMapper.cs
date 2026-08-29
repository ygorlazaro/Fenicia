using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.User;

public static partial class UserMapper
{
    public static UserListItemResponse MapToUserListItemResponse(this UserModel user)
    {
        return new UserListItemResponse(user.Id, user.Name, user.Email);
    }

    public static GetUserByIdResponse MapToGetUserByIdResponse(this UserModel user)
    {
        return new GetUserByIdResponse(user.Id, user.Name, user.Email);
    }

    public static GetByEmailResponse MapToGetByEmailResponse(this UserModel user)
    {
        return new GetByEmailResponse(user.Id, user.Email, user.Name, user.Password);
    }

    public static CreateUserResponse MapToCreateUserResponse(this UserModel user)
    {
        return new CreateUserResponse(user.Id, user.Name, user.Email);
    }

    public static UpdateUserResponse MapToUpdateUserResponse(this UserModel user)
    {
        return new UpdateUserResponse(user.Id, user.Name, user.Email);
    }

    public static UpdatePasswordResponse MapToUpdatePasswordResponse(this UserModel user)
    {
        return new UpdatePasswordResponse(user.Id, user.Name, user.Email);
    }

    public static GetUserForRefreshResponse MapToGetUserForRefreshResponse(this UserModel user)
    {
        return new GetUserForRefreshResponse(user.Id, user.Email, user.Name);
    }

    public static GetUserCompaniesResponse MapToGetUserCompaniesResponse(this UserRoleModel userRole)
    {
        return new GetUserCompaniesResponse(userRole.CompanyId, userRole.Role.Name, userRole.Company.Id, userRole.Company.Name, userRole.Company.Cnpj);
    }
}
