using Fenicia.Auth.Responses;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services.Interfaces;

public interface IModuleService
{
    Task<ApiResponse<List<ModuleResponse>>> GetAllOrderedAsync(int page = 1, int perPage = 10);
    Task<ApiResponse<List<ModuleResponse>>> GetModulesToOrderAsync(IEnumerable<Guid> request);
    Task<ApiResponse<ModuleResponse>> GetModuleByTypeAsync(ModuleType moduleType);
    Task<ApiResponse<int>> CountAsync();
}
