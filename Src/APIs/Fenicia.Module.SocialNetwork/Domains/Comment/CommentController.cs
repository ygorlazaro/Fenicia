using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Comment;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CommentController(
    CommentService commentService,
    ICompanyContext companyContext,
    IProfileService profileService) : ControllerBase
{
    [HttpGet("feed/{feedId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllCommentResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllCommentResponse>>> GetByFeedAsync(
        [FromRoute] Guid feedId,
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var result = await commentService.GetAllByFeedAsync(
            new GetAllCommentByFeedQuery(page, perPage, feedId, query, sort),
            feedId,
            profileId,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCommentByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetCommentByIdResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await commentService.GetByIdAsync(new GetCommentByIdQuery(id), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddCommentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddCommentResponse>> PostAsync(
        [FromBody] AddCommentCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var result = await commentService.AddAsync(
            command with { ProfileId = profileId },
            companyContext.CompanyId,
            profileId,
            cancellationToken);

        return new CreatedResult(string.Empty, result);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCommentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateCommentResponse>> PatchAsync(
        [FromBody] UpdateCommentCommand command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var result = await commentService.UpdateAsync(
            command with { Id = id },
            profileId,
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        await commentService.DeleteAsync(new DeleteCommentCommand(id), profileId, cancellationToken);

        return NoContent();
    }

    [HttpGet("replies/{parentCommentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetRepliesResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetRepliesResponse>>> GetRepliesAsync(
        [FromRoute] Guid parentCommentId,
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var result = await commentService.GetRepliesAsync(
            new GetRepliesQuery(page, perPage, parentCommentId, query, sort),
            profileId,
            cancellationToken);

        return Ok(result);
    }

    private async Task<Guid> GetCurrentProfileIdAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var profile = await profileService.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("Perfil social não encontrado para o usuário atual.");
        return profile.Id;
    }
}
