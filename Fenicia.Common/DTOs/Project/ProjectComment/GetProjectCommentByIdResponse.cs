using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectComment;

public class GetProjectCommentByIdResponse()
{
    public GetProjectCommentByIdResponse(
        [Required] Guid id,
        [Required] Guid taskId,
        [Required] Guid userId,
        [Required] string userName,
        [Required] [MaxLength(4096)] string content,
        DateTime created,
        [Required] Guid companyId)
        : this()
    {
        Id = id;
        TaskId = taskId;
        UserId = userId;
        UserName = userName;
        Content = content;
        Created = created;
        CompanyId = companyId;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(4096)]
    public string Content { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    [Required]
    public Guid CompanyId { get; set; }
}
