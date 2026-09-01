using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.SocialNetwork.Domains.Attachment;

public class AttachmentRepository(DefaultContext context) : Repository<AttachmentModel>(context);
