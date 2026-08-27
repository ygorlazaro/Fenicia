using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Employee.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Employee.DTOs.Queries;

public record GetAllEmployeeQuery(int Page = 1, int PerPage = 10);
