using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.SocialNetwork.Domains.Report.Interfaces;

namespace Fenicia.Module.SocialNetwork.Domains.Report;

public class ReportRepository(DefaultContext context) : Repository<ReportModel>(context), IReportRepository;