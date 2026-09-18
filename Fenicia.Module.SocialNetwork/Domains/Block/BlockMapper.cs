using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Block;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.SocialNetwork.Domains.Block;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class BlockMapper
{
    public partial AddBlockResponse MapToAddBlockResponse(BlockModel block);

    public partial GetBlockedResponse MapToGetBlockedResponse(BlockModel block);
}
