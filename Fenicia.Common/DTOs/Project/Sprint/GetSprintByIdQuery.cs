namespace Fenicia.Common.DTOs.Project.Sprint;

public class GetSprintByIdQuery()
{
    public GetSprintByIdQuery(Guid id)
        : this()
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
