using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Attachment;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.SocialNetwork.Domains.Attachment;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AttachmentMapper
{
    public partial AddAttachmentResponse MapToAddAttachmentResponse(AttachmentModel attachment);

    public partial GetAttachmentResponse MapToGetAttachmentResponse(AttachmentModel attachment);
}
