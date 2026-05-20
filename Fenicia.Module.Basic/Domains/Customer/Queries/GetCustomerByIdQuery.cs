using Fenicia.Module.Basic.Domains.Customer.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Customer.Queries;

/// <summary>
///     Query record for retrieving a specific customer by their unique identifier.
/// </summary>
public record GetCustomerByIdQuery(Guid Id) : IRequest<GetCustomerByIdResponse?>;
