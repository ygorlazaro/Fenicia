using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Subscription;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Subscription;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class SubscriptionMapper
{
    [MapProperty("Company.Id", nameof(UserCompanyResponse.Id))]
    [MapProperty("Company.Name", nameof(UserCompanyResponse.Name))]
    [MapProperty("Company.Cnpj", nameof(UserCompanyResponse.Cnpj))]
    public partial UserCompanyResponse MapToUserCompanyResponse(UserRoleModel userRole);

    [MapProperty("Company.Name", nameof(UserSubscriptionResponse.CompanyName))]
    public partial UserSubscriptionResponse MapToUserSubscriptionResponse(SubscriptionModel subscription);
}
