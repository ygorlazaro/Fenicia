using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services.Interfaces;

public interface ISubscriptionCreditService
{
    Task<List<ModuleType>> GetActiveModulesTypesAsync(Guid companyId);
}