using Fenicia.Auth.Domains.Subscription.DTOs;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Subscription;

public static partial class SubscriptionMapper
{
    public static UserCompanyResponse MapToUserCompanyResponse(this UserRoleModel userRole)
    {
        return new UserCompanyResponse(userRole.Company.Id, userRole.Company.Name, userRole.Company.Cnpj);
    }

    public static UserSubscriptionResponse MapToUserSubscriptionResponse(this SubscriptionModel subscription, string companyName, IEnumerable<UserModuleResponse> modules)
    {
        return new UserSubscriptionResponse(
            subscription.Id,
            subscription.CompanyId,
            companyName,
            subscription.Status,
            subscription.StartDate,
            subscription.EndDate)
        {
            Modules = modules
        };
    }

    public static UserModuleResponse MapToUserModuleResponse(this ModuleModel module)
    {
        return new UserModuleResponse(module.Id, module.Name, module.Type);
    }
}
