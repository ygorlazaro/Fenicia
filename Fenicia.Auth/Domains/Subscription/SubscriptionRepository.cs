using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Subscription;

public class SubscriptionRepository(DefaultContext context) : Repository<SubscriptionModel>(context)
{
}
