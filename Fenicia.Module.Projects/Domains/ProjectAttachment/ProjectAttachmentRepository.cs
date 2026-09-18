using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment;

public class ProjectAttachmentRepository(DbContext context) : Repository<AttachmentModel>(context),
    IProjectAttachmentRepository;