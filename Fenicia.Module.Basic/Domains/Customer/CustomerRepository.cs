using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Customer;

public class CustomerRepository(BasicContext context) : BaseRepository<CustomerModel>(context), ICustomerRepository
{
}
