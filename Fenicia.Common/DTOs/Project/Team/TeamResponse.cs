using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public class TeamResponse()
{
    public TeamResponse(
        [Required] Guid id,
        [Required] Guid projectId,
        [Required] [MaxLength(128)] string name,
        [MaxLength(2000)] string? description,
        [MaxLength(30)] string color,
        [Required] Guid createdBy,
        [Required] Guid companyId,
        int memberCount = 0,
        List<TeamMemberResponse>? members = null)
        : this()
    {
        Id = id;
        ProjectId = projectId;
        Name = name;
        Description = description;
        Color = color;
        CreatedBy = createdBy;
        CompanyId = companyId;
        MemberCount = memberCount;
        Members = members ?? [];
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(30)]
    public string Color { get; set; } = string.Empty;

    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    public int MemberCount { get; set; }

    public List<TeamMemberResponse> Members { get; set; } = [];
}