using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Configuration;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Configuration;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ConfigurationMapper
{
    internal partial ConfigurationResponse MapToConfigurationResponse(ConfigurationModel configuration);
}
