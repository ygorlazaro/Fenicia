using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;

public record GetAllProjectAttachmentQuery(int Page = 1, int PerPage = 10);