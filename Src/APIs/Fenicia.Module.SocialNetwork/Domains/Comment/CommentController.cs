using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Comment;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CommentController(CommentService commentService) : ControllerBase
{
    /// <summary>
    /// Gets all comments for a specific feed, ordered by comment date ascending.
    /// </summary>
    /// <param name="feedId">The unique identifier of the feed. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="page">Page number for pagination (1-based index). Example: <c>1</c></param>
    /// <param name="perPage">Number of items per page. Example: <c>10</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>A list of comments for the requested feed.</returns>
    /// <response code="200">Comments retrieved successfully.</response>
    /// <response code="400">Invalid pagination parameters supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while querying comments.</exception>
    [HttpGet("feed/{feedId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllCommentResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllCommentResponse>>> GetByFeedAsync([FromRoute] Guid feedId, WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await commentService.GetAllByFeedAsync(new GetAllCommentByFeedQuery(page, perPage, feedId), feedId, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets a comment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the comment. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The comment details, or null if not found.</returns>
    /// <response code="200">Comment found.</response>
    /// <response code="400">Invalid ID format supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="404">Comment with the specified ID was not found.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while querying the comment.</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCommentByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetCommentByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await commentService.GetByIdAsync(new GetCommentByIdQuery(id), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new comment.
    /// </summary>
    /// <param name="command">The comment data to create. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "userId": "22222222-2222-2222-2222-222222222222", "feedId": "33333333-3333-3333-3333-333333333333", "parentCommentId": null, "text": "Great post!" }</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The created comment details.</returns>
    /// <response code="201">Comment created successfully.</response>
    /// <response code="400">Invalid request body supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database insert.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while inserting the comment.</exception>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddCommentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddCommentResponse>> PostAsync([FromBody] AddCommentCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await commentService.AddAsync(command, ClaimReader.UserId(User), ClaimReader.UserId(User), cancellationToken);

        return new CreatedResult(string.Empty, result);
    }

    /// <summary>
    /// Updates an existing comment by its unique identifier.
    /// </summary>
    /// <param name="command">The comment data to update. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "text": "Updated comment text" }</c></param>
    /// <param name="id">The unique identifier of the comment to update. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The updated comment details, or null if the comment was not found or the user is not the owner.</returns>
    /// <response code="200">Comment updated successfully.</response>
    /// <response code="400">Invalid request body supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user is not the owner of the comment.</response>
    /// <response code="404">Comment with the specified ID was not found.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database update.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while updating the comment.</exception>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCommentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateCommentResponse>> PatchAsync([FromBody] UpdateCommentCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await commentService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Deletes a comment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to delete. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Comment deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="403">Forbidden - the user is not the owner of the comment.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database delete.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while deleting the comment.</exception>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await commentService.DeleteAsync(new DeleteCommentCommand(id), ClaimReader.UserId(User), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Gets all replies for a specific parent comment, ordered by comment date ascending.
    /// </summary>
    /// <param name="parentCommentId">The unique identifier of the parent comment. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="page">Page number for pagination (1-based index). Example: <c>1</c></param>
    /// <param name="perPage">Number of items per page. Example: <c>10</c></param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>A list of replies for the requested parent comment.</returns>
    /// <response code="200">Replies retrieved successfully.</response>
    /// <response code="400">Invalid pagination parameters supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while querying replies.</exception>
    [HttpGet("replies/{parentCommentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetRepliesResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetRepliesResponse>>> GetRepliesAsync([FromRoute] Guid parentCommentId, WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await commentService.GetRepliesAsync(new GetRepliesQuery(page, perPage, parentCommentId), cancellationToken);

        return Ok(result);
    }
}
