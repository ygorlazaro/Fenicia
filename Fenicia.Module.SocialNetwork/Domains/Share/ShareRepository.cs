using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.SocialNetwork.Domains.Share.Interfaces;

namespace Fenicia.Module.SocialNetwork.Domains.Share;

public class ShareRepository(DefaultContext context) : Repository<ShareModel>(context), IShareRepository;