using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.SocialNetwork.Domains.Share.Interfaces;

/// <summary>
/// Fenicia.Module.SocialNetwork/Domains/Share/ShareRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.SocialNetwork.Domains.Share;

public class ShareRepository(DefaultContext context) : Repository<ShareModel>(context), IShareRepository;