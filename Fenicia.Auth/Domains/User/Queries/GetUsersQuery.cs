using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common;

using MediatR;

namespace Fenicia.Auth.Domains.User.Queries;

public record GetUsersQuery(int Page = 1, int PerPage = 10) : IRequest<Pagination<List<UserListItemResponse>>>;
