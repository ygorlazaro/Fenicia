using Fenicia.Module.Basic.Domains.Order.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Order.Queries;

public record GetOrderAnalyticsQuery(int Days = 90, int TopCustomersLimit = 10) : IRequest<OrderAnalyticsResponse>;
