using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.User;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.User;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class UserMapper
{
    public partial UserListItemResponse MapToUserListItemResponse(UserModel user);

    public partial GetUserByIdResponse MapToGetUserByIdResponse(UserModel user);

    public partial GetByEmailResponse MapToGetByEmailResponse(UserModel user);

    public partial CreateUserResponse MapToCreateUserResponse(UserModel user);

    public partial UpdateUserResponse MapToUpdateUserResponse(UserModel user);

    public partial UpdatePasswordResponse MapToUpdatePasswordResponse(UserModel user);

    public partial GetUserForRefreshResponse MapToGetUserForRefreshResponse(UserModel user);
}
