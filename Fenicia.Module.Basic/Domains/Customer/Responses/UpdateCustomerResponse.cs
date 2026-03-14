namespace Fenicia.Module.Basic.Domains.Customer.Responses;

/// <summary>
/// Response model returned after successfully updating a customer.
/// </summary>
public record UpdateCustomerResponse(Guid Id, Guid PersonId);
