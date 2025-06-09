using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services.Interfaces;

public interface ISubscriptionCreditService
{
    Task<ServiceResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId);
}
