using System.Net.Mime;

using Fenicia.Auth.Domains.Company.Commands;

using Fenicia.Auth.Domains.Company.Queries;
using Fenicia.Auth.Domains.Company.Responses;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Company;

/// <summary>
///     Controller responsible for handling company-related HTTP endpoints.
///     Provides endpoints to retrieve companies associated with the logged-in user
///     and to update company information.
/// </summary>
/// <remarks>
///     All endpoints require authentication. The Update endpoint requires Admin role.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CompanyController(ISender sender) : ControllerBase
{
    /// <summary>
    ///     Retrieves a paginated list of companies associated with the currently logged-in user.
    /// </summary>
    /// <param name="query">Pagination query parameters (page number and items per page).</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing companies the user is associated with.</returns>
    /// <response code="200">Returns the list of companies successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="400">Invalid request - perPage must be greater than zero.</response>
    /// <response code="500">Internal server error.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    /// <exception cref="InvalidRequestException">Invalid request - perPage must be greater than zero.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GetCompaniesByUserResponse>> GetByLoggedUser([FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var getCompaniesByUserQuery = new GetCompaniesByUserQuery(userId, query.Page, query.PerPage);
            var result = await sender.Send(getCompaniesByUserQuery, ct);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    ///     Updates company information.
    /// </summary>
    /// <param name="id">The unique identifier of the company to update.</param>
    /// <param name="request">The update command containing the new company name.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on successful update.</returns>
    /// <response code="204">Company updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">User does not have Admin permission for this company.</response>
    /// <response code="404">Company not found.</response>
    /// <response code="500">Internal server error.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    /// <exception cref="ItemNotExistsException">Company not found or inactive.</exception>
    /// <exception cref="PermissionDeniedException">User does not have Admin permission for this company.</exception>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchAsync([FromRoute] Guid id, [FromBody] UpdateCompanyCommand request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var company = request with { CompanyId = id };

            await sender.Send(company, ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (ItemNotExistsException ex)
        {
            return NotFound(ex.Message);
        }
        catch (PermissionDeniedException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
