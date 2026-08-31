using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.SocialNetwork.Domains.Report;

public class ReportRepository(DefaultContext context) : Repository<ReportModel>(context)
{
}
