using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Common;

using MediatR;

namespace Fenicia.Auth.Domains.Module.Queries;

/// <summary>
///     Query to retrieve a paginated list of available modules.
/// </summary>
/// <remarks>
///     Used by GetModulesHandler to fetch modules for public display.
///     Default pagination is page 1 with 20 items per page.
/// </remarks>
public sealed record GetModulesQuery(int Page = 1, int PerPage = 20) : IRequest<Pagination<List<GetModuleResponse>>>;
