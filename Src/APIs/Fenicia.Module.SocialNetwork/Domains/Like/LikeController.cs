using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Like.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Like;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class LikeController(LikeService likeService) : ControllerBase
{
    /// <summary>
    ///     Creates a like for a feed.
    /// </summary>
    /// <param name="command">
    ///     The like data to create. Example: <c>{ "feedId": "11111111-1111-1111-1111-111111111111" }</c>
    /// </param>
    /// <param name="wide">
    ///     Wide event context for audit logging. Example:
    ///     <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c>
    /// </param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The created like details.</returns>
    /// <response code="201">Like created successfully.</response>
    /// <response code="400">Invalid request body supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">
    ///     Thrown by the repository when the cancellation token is triggered during
    ///     the database insert.
    /// </exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while inserting the like.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddLikeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddLikeResponse>> PostAsync(
        [FromBody] LikeCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await likeService.LikeAsync(
            command,
            ClaimReader.UserId(User),
            ClaimReader.UserId(User),
            cancellationToken);

        return new CreatedResult(string.Empty, result);
    }

    /// <summary>
    ///     Removes a like from a feed.
    /// </summary>
    /// <param name="feedId">
    ///     The unique identifier of the feed to unlike. Example: <c>11111111-1111-1111-1111-111111111111</c>
    /// </param>
    /// <param name="wide">
    ///     Wide event context for audit logging. Example:
    ///     <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c>
    /// </param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Like removed successfully.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user does not have the Admin role required to unlike feeds.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">
    ///     Thrown by the repository when the cancellation token is triggered during
    ///     the database delete.
    /// </exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while deleting the like.</exception>
    [HttpDelete("{feedId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UnlikeAsync(
        [FromRoute] Guid feedId,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await likeService.UnlikeAsync(new UnlikeCommand(feedId), ClaimReader.UserId(User), cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Gets all likes for a specific feed.
    /// </summary>
    /// <param name="feedId">The unique identifier of the feed. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">
    ///     Wide event context for audit logging. Example:
    ///     <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c>
    /// </param>
    /// <param name="page">Page number for pagination (1-based index). Example: <c>1</c></param>
    /// <param name="perPage">Number of items per page. Example: <c>10</c></param>
    /// <param name="sort"></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <param name="query"></param>
    /// <returns>A list of likes for the requested feed.</returns>
    /// <response code="200">Likes retrieved successfully.</response>
    /// <response code="400">Invalid pagination parameters supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">
    ///     Thrown by the repository when the cancellation token is triggered during
    ///     the database query.
    /// </exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while querying likes.</exception>
    [HttpGet("feed/{feedId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetLikesResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetLikesResponse>>> GetLikesByFeedAsync(
        [FromRoute] Guid feedId,
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await likeService.GetLikesByFeedAsync(
            new GetLikesByFeedQuery(page, perPage, feedId, query, sort),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    ///     Checks if a specific user has liked a specific feed.
    /// </summary>
    /// <param name="userId">The unique identifier of the user. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="feedId">The unique identifier of the feed. Example: <c>22222222-2222-2222-2222-222222222222</c></param>
    /// <param name="wide">
    ///     Wide event context for audit logging. Example:
    ///     <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c>
    /// </param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>True if the user has liked the feed, otherwise false.</returns>
    /// <response code="200">Like status retrieved successfully.</response>
    /// <response code="400">Invalid ID format supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">
    ///     Thrown by the repository when the cancellation token is triggered during
    ///     the database query.
    /// </exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while checking like status.</exception>
    [HttpGet("isfollowed/{userId:guid}/{feedId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> IsLikedAsync(
        [FromRoute] Guid userId,
        [FromRoute] Guid feedId,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await likeService.IsLikedAsync(new IsLikedQuery(), userId, feedId, cancellationToken);

        return Ok(result);
    }
}