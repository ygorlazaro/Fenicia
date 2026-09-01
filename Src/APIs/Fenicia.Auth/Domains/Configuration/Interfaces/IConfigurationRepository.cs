using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Configuration.Interfaces;

public interface IConfigurationRepository : IRepository<ConfigurationModel>
{
    Task<ConfigurationModel?> GetByUserCompanyAndTypeAsync(Guid userId, Guid companyId, ConfigType configType, CancellationToken cancellationToken = default);

    Task<List<ConfigurationModel>> GetByUserAndCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default);
}
