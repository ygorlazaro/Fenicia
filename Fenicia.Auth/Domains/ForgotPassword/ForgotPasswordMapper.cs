using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.ForgotPassword;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.ForgotPassword;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Source)]
public partial class ForgotPasswordMapper
{
    [MapperIgnoreTarget(nameof(ForgotPasswordModel.Code))]
    [MapperIgnoreTarget(nameof(ForgotPasswordModel.ExpirationDate))]
    [MapperIgnoreTarget(nameof(ForgotPasswordModel.Id))]
    [MapperIgnoreTarget(nameof(ForgotPasswordModel.User))]
    [MapperIgnoreSource(nameof(ForgotPasswordRequest.Email))]
    internal partial ForgotPasswordModel MapToForgotPasswordModel(ForgotPasswordRequest request);
}
