using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Handlers;
using Fenicia.Module.Basic.Domains.Customer.Queries;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Customer;

/// <summary>
/// Controller responsible for handling customer-related HTTP endpoints.
/// Provides CRUD operations and customer analytics.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CustomerController(
    GetAllCustomerHandler getAllCustomerHandler,
    GetCustomerByIdHandler getCustomerByIdHandler,
    AddCustomerHandler addCustomerHandler,
    UpdateCustomerHandler updateCustomerHandler,
    DeleteCustomerHandler deleteCustomerHandler,
    GetCustomerInsightsHandler getCustomerInsightsHandler) : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of all customers.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated list of customers.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllCustomerResponse>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllCustomerResponse>>>> GetAsync(
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var customers = await getAllCustomerHandler.Handle(new GetAllCustomerQuery(page,
                perPage),
            ct);

        return Ok(customers);
    }

    /// <summary>
    /// Retrieves a specific customer by ID.
    /// </summary>
    /// <param name="id">Customer's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Customer details or 404 if not found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCustomerByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetCustomerByIdResponse>> GetByIdAsync(
        [FromRoute] Guid id, 
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var customer = await getCustomerByIdHandler.Handle(new GetCustomerByIdQuery(id),
            ct);

        return customer is null ? NotFound() : Ok(customer);
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="command">Customer creation command.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created customer.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddCustomerResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddCustomerResponse>> PostAsync(
        [FromBody] AddCustomerCommand command, 
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var customer = await addCustomerHandler.Handle(command,
            ct);

        return new CreatedResult(string.Empty,
            customer);
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="command">Customer update command.</param>
    /// <param name="id">Customer's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated customer or 404 if not found.</returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCustomerResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateCustomerResponse>> PatchAsync(
        [FromBody] UpdateCustomerCommand command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var customer = await updateCustomerHandler.Handle(command with
            {
                Id = id
            },
            ct);

        return customer is null ? NotFound() : Ok(customer);
    }

    /// <summary>
    /// Deletes a customer (soft delete).
    /// </summary>
    /// <param name="id">Customer's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id, 
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        await deleteCustomerHandler.Handle(new DeleteCustomerCommand(id),
            ct);

        return NoContent();
    }

    /// <summary>
    /// Retrieves customer analytics and insights.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="days">Number of days to analyze (default: 90).</param>
    /// <param name="topLimit">Number of top customers to return (default: 10).</param>
    /// <param name="riskThresholdDays">Days threshold for risk alerts (default: 60).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Customer insights including top customers and risk alerts.</returns>
    [HttpGet("insights")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerInsightsResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerInsightsResponse>> GetInsightsAsync(
        WideEventContext wide,
        [FromQuery] int days = 90,
        [FromQuery] int topLimit = 10,
        [FromQuery] int riskThresholdDays = 60,
        CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var insights = await getCustomerInsightsHandler.Handle(new GetCustomerInsightsQuery(days,
                topLimit,
                riskThresholdDays),
            ct);

        return Ok(insights);
    }
}
