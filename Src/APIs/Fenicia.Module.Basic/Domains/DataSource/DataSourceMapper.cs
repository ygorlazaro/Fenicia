using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.DataSource;

[Mapper]
public static partial class DataSourceMapper
{
    public static partial GetAllPositionForDataSourceResponse MapToDataSourceResponse(this GetAllPositionResponse position);

    public static partial GetAllProductCategoryForDataSourceResponse MapToDataSourceResponse(this GetAllProductCategoryResponse category);
}
