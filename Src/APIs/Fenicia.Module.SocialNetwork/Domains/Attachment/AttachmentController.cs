using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Attachment;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class AttachmentController(AttachmentService attachmentService) : ControllerBase
{
    /// <summary>
    /// Creates a new attachment.
    /// </summary>
    /// <param name="command">Attachment data. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "url": "https://example.com/file.pdf", "fileType": "pdf", "fileSize": 1024, "commentId": "22222222-2222-2222-2222-222222222222" }</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="ct">Cancellation token to cancel the request.</param>
    /// <returns>The created attachment details.</returns>
    /// <response code="201">Attachment created successfully. Example: <c>{ "id": "11111111-1111-1111-1111-111111111111", "url": "https://example.com/file.pdf", "fileType": "pdf", "fileSize": 1024, "commentId": "22222222-2222-2222-2222-222222222222", "companyId": "33333333-3333-3333-3333-333333333333", "uploadDate": "2024-01-15T00:00:00Z" }</c></response>
    /// <response code="400">Invalid request body supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database insert.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while inserting the attachment.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddAttachmentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddAttachmentResponse>> PostAsync([FromBody] AddAttachmentCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var attachment = await attachmentService.AddAsync(command, ClaimReader.UserId(User), ClaimReader.UserId(User), ct);

        return new CreatedResult(string.Empty, attachment);
    }

    /// <summary>
    /// Deletes an attachment by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the attachment to delete. Example: <c>11111111-1111-1111-1111-111111111111</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="ct">Cancellation token to cancel the request.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Attachment deleted successfully.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database delete.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while deleting the attachment.</exception>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await attachmentService.DeleteAsync(new DeleteAttachmentCommand(id), ClaimReader.UserId(User), ct);

        return NoContent();
    }

    /// <summary>
    /// Gets all attachments for a specific comment with pagination.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment. Example: <c>22222222-2222-2222-2222-222222222222</c></param>
    /// <param name="wide">Wide event context for audit logging. Example: <c>{ "userId": "11111111-1111-1111-1111-111111111111" }</c></param>
    /// <param name="page">Page number for pagination (1-based index). Example: <c>1</c></param>
    /// <param name="perPage">Number of items per page. Example: <c>10</c></param>
    /// <param name="ct">Cancellation token to cancel the request.</param>
    /// <returns>A list of attachments for the requested comment.</returns>
    /// <response code="200">Attachments retrieved successfully. Example: <c>[{ "id": "11111111-1111-1111-1111-111111111111", "url": "https://example.com/file.pdf", "fileType": "pdf", "fileSize": 1024, "commentId": "22222222-2222-2222-2222-222222222222", "uploadDate": "2024-01-15T00:00:00Z" }]</c></response>
    /// <response code="400">Invalid pagination parameters supplied.</response>
    /// <response code="401">Unauthorized - authentication is required.</response>
    /// <response code="500">Internal server error caused by a database failure.</response>
    /// <exception cref="OperationCanceledException">Thrown by the repository when the cancellation token is triggered during the database query.</exception>
    /// <exception cref="DbUpdateException">Thrown by the repository when a database error occurs while querying attachments.</exception>
    [HttpGet("comment/{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAttachmentResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAttachmentResponse>>> GetByCommentAsync([FromRoute] Guid commentId, WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var attachments = await attachmentService.GetByCommentAsync(new GetAttachmentsByCommentQuery(page, perPage), commentId, ct);

        return Ok(attachments);
    }
}
