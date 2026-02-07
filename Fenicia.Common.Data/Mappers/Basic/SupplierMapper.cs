using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class SupplierMapper
{
    public static SupplierModel Map(SupplierRequest request)
    {
        return new SupplierModel
        {
            Id = request.Id,
            Person = PersonMapper.Map(request.Person)
        };
    }

    public static SupplierResponse Map(SupplierModel model)
    {
        return new SupplierResponse
        {
            Id = model.Id,
            Person = PersonMapper.Map(model.Person)
        };
    }

    public static List<SupplierResponse> Map(List<SupplierModel> suppliers)
    {
        return [.. suppliers.Select(Map)];
    }
}