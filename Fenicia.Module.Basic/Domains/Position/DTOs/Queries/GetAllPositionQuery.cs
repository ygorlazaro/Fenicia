using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Position.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Position.DTOs.Queries;

public record GetAllPositionQuery(int Page = 1, int PerPage = 10);
