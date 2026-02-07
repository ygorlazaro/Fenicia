using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class CustomerMapper
{
    public static CustomerResponse Map(CustomerModel model)
    {
        return new CustomerResponse
        {
            Person = PersonMapper.Map(model.Person),
            Id = model.Id
        };
    }

    public static CustomerModel Map(CustomerRequest request)
    {
        return new CustomerModel
        {
            Person = PersonMapper.Map(request.Person),
            Id = request.Id,
        };
    }

    public static List<CustomerResponse> Map(List<CustomerModel> model)
    {
        return [.. model.Select(Map)];
    }
}
