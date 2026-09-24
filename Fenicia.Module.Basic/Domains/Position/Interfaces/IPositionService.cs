using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.Position;

namespace Fenicia.Module.Basic.Domains.Position.Interfaces;

/// <summary>
/// Service interface for position operations.
/// </summary>
public interface IPositionService
{
    /// <summary>
    /// Gets a paginated list of positions.
    /// </summary>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A paginated list of position responses.</returns>
    Task<Pagination<List<PositionResponse>>> GetAllAsync(
        PositionQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a position by ID.
    /// </summary>
    /// <param name="id">The position ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The position response if found, or null.</returns>
    Task<PositionResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new position.
    /// </summary>
    /// <param name="command">The position request data.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created position response.</returns>
    Task<PositionResponse> AddAsync(
        PositionRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing position.
    /// </summary>
    /// <param name="id">The position ID.</param>
    /// <param name="command">The position request data.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The updated position response if found, or null.</returns>
    Task<PositionResponse?> UpdateAsync(
        Guid id,
        PositionRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a position.
    /// </summary>
    /// <param name="id">The position ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);
}