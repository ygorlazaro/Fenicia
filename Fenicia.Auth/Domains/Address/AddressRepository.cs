using Fenicia.Auth.Domains.Address.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Address;

/// <summary>
/// Repository implementation for managing address data.
/// </summary>
/// <param name="context">The database context.</param>
public class AddressRepository(DbContext context) : Repository<AddressModel>(context), IAddressRepository;