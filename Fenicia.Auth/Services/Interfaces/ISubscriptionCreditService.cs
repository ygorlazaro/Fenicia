using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services.Interfaces;

public interface ISubscriptionCreditService
{
    Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId);
}
