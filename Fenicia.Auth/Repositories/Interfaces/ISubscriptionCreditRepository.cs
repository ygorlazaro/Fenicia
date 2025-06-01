using Fenicia.Common.Enums;

namespace Fenicia.Auth.Repositories.Interfaces;

public interface ISubscriptionCreditRepository
{
    Task<List<ModuleType>> GetValidModulesTypesAsync(List<Guid> subscriptions);
}