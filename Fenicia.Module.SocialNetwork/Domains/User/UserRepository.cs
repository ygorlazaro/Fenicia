using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.User;

public class UserRepository(SocialNetworkContext context) : BaseRepository<UserModel>(context), IUserRepository
{

}