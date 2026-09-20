using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.PersonAddress;

public class PersonAddressRepository(DbContext context)
    : Repository<PersonAddressModel>(context), IPersonAddressRepository;