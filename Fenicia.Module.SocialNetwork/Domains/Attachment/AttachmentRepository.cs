using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.SocialNetwork.Domains.Attachment.Interfaces;

/// <summary>
/// Fenicia.Module.SocialNetwork/Domains/Attachment/AttachmentRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.SocialNetwork.Domains.Attachment;

public class AttachmentRepository(DefaultContext context) : Repository<AttachmentModel>(context), IAttachmentRepository;