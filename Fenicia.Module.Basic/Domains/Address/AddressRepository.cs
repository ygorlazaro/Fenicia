using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Address;

public class AddressRepository(DbContext context) : Repository<AddressModel>(context), IAddressRepository;