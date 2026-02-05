using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Customer;

public class CustomerRepository(BasicContext context) : BaseRepository<CustomerModel>(context), ICustomerRepository
{
}
