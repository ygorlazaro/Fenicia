using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class FeedController(FeedService feedService) : ControllerBase
{
    /// <summary>
    /// Gets all feeds with pagination, ordered by date descending.
    /// </summary>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="page">Page number for pagination (1-based index). Example: <c>1</c></param>
    /// <param name="perPage">Number of items per page. Example: <c>10</c></param>
    /// <param name="sort"></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <param name="query"></param>
    /// <returns>A list of feeds for the requested page.</returns>
    /// <response code="200">Feeds retrieved successfully.</response>
    /// <response code="400">Invalid pagination parameters supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while querying feeds.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllFeedResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllFeedResponse>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, [FromQuery] string? query = null, [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await feedService.GetAllAsync(new GetAllFeedQuery(page, perPage, query, sort), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets a feed by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the feed. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The feed details, or null if not found.</returns>
    /// <response code="200">Feed found.</response>
    /// <response code="400">Invalid ID format supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="404">Feed with the specified ID was not found.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while querying the feed.</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFeedByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetFeedByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await feedService.GetByIdAsync(new GetFeedByIdQuery(id), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new feed.
    /// </summary>
    /// <param name="command">The feed data to create. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "date": "2024-01-15T00:00:00Z", "text": "Hello world", "userId": "22222222-2222-2222-2222-222222222222" }</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The created feed details.</returns>
    /// <response code="201">Feed created successfully.</response>
    /// <response code="400">Invalid request body supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user does not have the Admin role required to create feeds.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database insert.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while inserting the feed.</exception>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddFeedResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddFeedResponse>> PostAsync([FromBody] AddFeedCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await feedService.AddAsync(command, ClaimReader.UserId(User), cancellationToken);

        return new CreatedResult(string.Empty, result);
    }

    /// <summary>
    /// Updates an existing feed by its unique identifier.
    /// </summary>
    /// <param name="command">The feed data to update. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "date": "2024-01-15T00:00:00Z", "text": "Updated text" }</c></param>
    /// <param name="id">The unique identifier of the feed to update. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The updated feed details, or null if the feed was not found.</returns>
    /// <response code="200">Feed updated successfully.</response>
    /// <response code="400">Invalid request body supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user does not have the Admin role required to update feeds.</response>
    /// <response code="404">Feed with the specified ID was not found.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database update.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while updating the feed.</exception>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateFeedResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateFeedResponse>> PatchAsync([FromBody] UpdateFeedCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await feedService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Deletes a feed by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the feed to delete. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Feed deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user does not have the Admin role required to delete feeds.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database delete.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while deleting the feed.</exception>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await feedService.DeleteAsync(new DeleteFeedCommand(id), cancellationToken);

        return NoContent();
    }
}
