using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Share;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.SocialNetwork.Domains.Share;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ShareMapper
{
    public partial GetSharesResponse MapToGetSharesResponse(ShareModel share);
}
