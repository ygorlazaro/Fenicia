namespace Fenicia.Auth.Domains.Module.Logic;

using System.ComponentModel.DataAnnotations;

using Common.Enums;

using Data;

/// <summary>
///     Repository interface for managing module-related data operations
/// </summary>
public interface IModuleRepository
{
    /// <summary>
    ///     Retrieves a paginated list of all modules ordered by a default criterion
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <param name="page">The page number to retrieve (1-based)</param>
    /// <param name="perPage">Number of items per page</param>
    /// <returns>A list of module models for the specified page</returns>
    /// <exception cref="ArgumentException">Thrown when page or perPage parameters are invalid</exception>
    Task<List<ModuleModel>> GetAllOrderedAsync(CancellationToken cancellationToken, [Range(minimum: 1, int.MaxValue)] int page = 1, [Range(minimum: 1, maximum: 100)] int perPage = 10);

    /// <summary>
    ///     Retrieves multiple modules by their identifiers
    /// </summary>
    /// <param name="request">Collection of module identifiers to retrieve</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>A list of found module models</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    Task<List<ModuleModel>> GetManyOrdersAsync([Required] IEnumerable<Guid> request, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a module by its type
    /// </summary>
    /// <param name="moduleType">The type of module to retrieve</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>The module model if found, null otherwise</returns>
    Task<ModuleModel?> GetModuleByTypeAsync(ModuleType moduleType, CancellationToken cancellationToken);

    /// <summary>
    ///     Counts the total number of modules in the system
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>The total count of modules</returns>
    Task<int> CountAsync(CancellationToken cancellationToken);
}
