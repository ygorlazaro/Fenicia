using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Share.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Share;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ShareController(ShareService shareService) : ControllerBase
{
    /// <summary>
    /// Creates a new share of a feed.
    /// </summary>
    /// <param name="command">Share data. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "originalFeedId": "22222222-2222-2222-2222-222222222222", "text": "Check this out!" }</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The created share details.</returns>
    /// <response code="201">Share created successfully. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "originalFeedId": "22222222-2222-2222-2222-222222222222", "text": "Check this out!", "companyId": "33333333-3333-3333-3333-333333333333", "userId": "11111111-1111-1111-1111-111111111111", "shareDate": "2024-01-15T00:00:00Z" }</c></response>
    /// <response code="400">Invalid request body supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database insert.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddShareResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddShareResponse>> PostAsync([FromBody] ShareCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var share = await shareService.ShareAsync(command, ClaimReader.UserId(User), ClaimReader.UserId(User), cancellationToken);

        return new CreatedResult(string.Empty, share);
    }

    /// <summary>
    /// Gets all shares for a specific feed with pagination.
    /// </summary>
    /// <param name="feedId">The unique identifier of the feed. Example: <c>22222222-2222-2222-2222-222222222222</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="page">Page number for pagination (1-based index). Example: <c>1</c></param>
    /// <param name="perPage">Number of items per page. Example: <c>10</c></param>
    /// <param name="query"></param>
    /// <param name="sort"></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>A list of shares for the requested feed.</returns>
    /// <response code="200">Shares retrieved successfully. Example: <c>[{ "id": "11111111-1111-1111-1111-111111111111", "originalFeedId": "22222222-2222-2222-2222-222222222222", "text": "Check this out!", "companyId": "33333333-3333-3333-3333-333333333333", "userId": "11111111-1111-1111-1111-111111111111", "shareDate": "2024-01-15T00:00:00Z" }]</c></response>
    /// <response code="400">Invalid pagination parameters supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    [HttpGet("feed/{feedId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetSharesResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetSharesResponse>>> GetSharesByFeedAsync([FromRoute] Guid feedId, WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, [FromQuery] string? query = null, [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var shares = await shareService.GetSharesByFeedAsync(new GetSharesByFeedQuery(page, perPage, query, sort), feedId, cancellationToken);

        return Ok(shares);
    }
}
