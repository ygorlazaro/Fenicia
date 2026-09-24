using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Customer;

/// <summary>
/// Controller responsible for handling customer-related operations.
/// </summary>
/// <param name="customerService">The customer service.</param>
/// <param name="companyContext">The company context.</param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CustomerController(ICustomerService customerService, ICompanyContext companyContext) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of customers.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="query">Pagination query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of customers.</returns>
    /// <response code="200">List of customers returned successfully.</response>
    /// <response code="400">Invalid ID.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Pagination<List<CustomerResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<CustomerResponse>>>> GetAsync(
        WideEventContext wide,
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var customers = await customerService.GetAllAsync(query, cancellationToken);

        return Ok(customers);
    }

    /// <summary>
    /// Gets a customer by ID.
    /// </summary>
    /// <param name="id">Customer ID.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Customer details.</returns>
    /// <response code="200">Customer found.</response>
    /// <response code="400">Invalid ID.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Customer not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var customer = await customerService.GetByIdAsync(id, cancellationToken);

        return customer is null ? NotFound() : Ok(customer);
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="request">Customer data to create.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created customer.</returns>
    /// <response code="201">Customer created successfully.</response>
    /// <response code="400">Invalid data.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPost]
    [Authorize(Roles = "Admin,God")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CustomerResponse>> PostAsync(
        [FromBody] CustomerRequest request,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var customer = await customerService.AddAsync(request, companyContext.CompanyId, cancellationToken);

        return new CreatedResult(string.Empty, customer);
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="id">Customer ID.</param>
    /// <param name="request">Updated customer data.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated customer.</returns>
    /// <response code="200">Customer updated successfully.</response>
    /// <response code="400">Invalid data.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Customer not found.</response>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin,God")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CustomerResponse>> PatchAsync(
        [FromRoute] Guid id,
        [FromBody] CustomerRequest request,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var customer = await customerService.UpdateAsync(id, request, companyContext.CompanyId, cancellationToken);

        return customer switch
        {
            null => NotFound(),
            _ => Ok(customer)
        };
    }

    /// <summary>
    /// Deletes a customer (soft delete).
    /// </summary>
    /// <param name="id">Customer ID.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Customer deleted successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,God")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await customerService.DeleteAsync(id, companyContext.CompanyId, cancellationToken);

        return NoContent();
    }
}