using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Position;
using Fenicia.Common.DTOs.Basic.ProductCategory;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.DataSource;

[Mapper]
public static partial class DataSourceMapper
{
    public static partial GetAllPositionForDataSourceResponse MapToDataSourceResponse(
        this GetAllPositionResponse position);

    public static partial GetAllProductCategoryForDataSourceResponse MapToDataSourceResponse(
        this GetAllProductCategoryResponse category);
}