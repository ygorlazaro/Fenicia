namespace Fenicia.Common.DTOs.Project.Sprint;

public class AddSprintResponse()
{
    public AddSprintResponse(
        Guid id,
        Guid projectId,
        string name,
        DateTime? startDate,
        DateTime? endDate,
        string? description,
        Guid createdBy,
        Guid companyId)
        : this()
    {
        Id = id;
        ProjectId = projectId;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
        CreatedBy = createdBy;
        CompanyId = companyId;
    }

    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid CompanyId { get; set; }
}
