using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment;

public class ProjectAttachmentRepository(DefaultContext context) : Repository<AttachmentModel>(context);
