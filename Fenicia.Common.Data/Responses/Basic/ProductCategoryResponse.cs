using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class ProductCategoryResponse(ProductCategoryModel model)
{
    public Guid Id { get; set; } = model.Id;

    public string Name { get; set; } = model.Name;
}