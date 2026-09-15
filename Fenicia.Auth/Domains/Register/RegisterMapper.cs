using Fenicia.Common.DTOs.Auth.Register;
using Fenicia.Common.DTOs.Auth.User;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Register;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class RegisterMapper
{
    internal partial RegisterResponse MapToRegisterResponse(CreateNewUserResponse userResponse);
}
