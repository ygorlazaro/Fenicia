using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Project;

public class GetAllProjectQuery()
{
    public GetAllProjectQuery(int page = 1, int perPage = 10)
        : this()
    {
        Page = page;
        PerPage = perPage;
    }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PerPage { get; set; } = 10;
}