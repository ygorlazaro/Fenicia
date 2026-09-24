using System.Net.Mime;
using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Auth.Company;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Company;

/// <summary>
/// Controller responsible for handling company-related operations, including retrieving companies associated with the authenticated user and updating company information.
/// </summary>
/// <param name="service">The company service.</param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CompanyController(ICompanyService service) : ControllerBase
{
    /// <summary>
    ///    Retrieves a paginated list of companies associated with the currently authenticated user.
    /// </summary>
    /// <param name="query">The pagination query parameters.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The paginated list of companies.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<IEnumerable<CompanyResponse>>>> GetByLoggedUser(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            var result = await service.GetCompaniesByUserAsync(userId, query.Page, query.PerPage, cancellationToken);

            return Ok(result);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates the name of an existing company, ensuring that the user has administrative privileges for that company.
    /// </summary>
    /// <param name="id">The ID of the company to update.</param>
    /// <param name="request">The request containing the updated company name.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>No content if the update is successful.</returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchAsync(
        [FromRoute] Guid id,
        [FromBody] CompanyRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);

            await service.UpdateAsync(id, userId, request.Name, cancellationToken);

            return NoContent();
        }
        catch (ForbiddenException ex)
        {
            return NotFound(ex.Message);
        }
        catch (PermissionDeniedException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
