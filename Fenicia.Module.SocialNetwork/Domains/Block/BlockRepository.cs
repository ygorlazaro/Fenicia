using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.SocialNetwork.Domains.Block.Interfaces;

/// <summary>
/// Fenicia.Module.SocialNetwork/Domains/Block/BlockRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.SocialNetwork.Domains.Block;

public class BlockRepository(DefaultContext context) : Repository<BlockModel>(context), IBlockRepository
{
    public override IQueryable<BlockModel> Query()
    {
        return DbSet;
    }
}
