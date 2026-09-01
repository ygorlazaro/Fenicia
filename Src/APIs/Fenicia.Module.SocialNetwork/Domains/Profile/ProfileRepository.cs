using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.SocialNetwork.Domains.Profile;

public interface IProfileRepository : IRepository<ProfileModel>;

public class ProfileRepository(DefaultContext context) : Repository<ProfileModel>(context), IProfileRepository;
