using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.SocialNetwork.Domains.Block;

public interface IBlockRepository : IRepository<BlockModel>
{
    new IQueryable<BlockModel> Query();
}