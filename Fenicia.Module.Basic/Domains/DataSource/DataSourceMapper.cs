using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Position;
using Fenicia.Common.DTOs.Basic.ProductCategory;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.DataSource;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class DataSourceMapper
{
    internal partial GetAllPositionForDataSourceResponse MapToDataSourceResponse(GetAllPositionResponse position);

    internal partial GetAllProductCategoryForDataSourceResponse MapToDataSourceResponse(
        GetAllProductCategoryResponse category);
}
