namespace Fenicia.Module.Basic.Domains.Customer.Responses;

/// <summary>
/// Response model returned after successfully creating a new customer.
/// </summary>
public record AddCustomerResponse(Guid Id, Guid PersonId);
