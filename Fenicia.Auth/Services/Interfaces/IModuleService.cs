using Fenicia.Auth.Responses;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services.Interfaces;

public interface IModuleService
{
    Task<ServiceResponse<List<ModuleResponse>>> GetAllOrderedAsync(int page = 1, int perPage = 10);
    Task<ServiceResponse<List<ModuleResponse>>> GetModulesToOrderAsync(IEnumerable<Guid> request);
    Task<ServiceResponse<ModuleResponse>> GetModuleByTypeAsync(ModuleType moduleType);
    Task<ServiceResponse<int>> CountAsync();
}