using Fenicia.Auth.Domains.PersonAddress.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.PersonAddress;

/// <summary>
/// Repository implementation for managing person address relationship data.
/// </summary>
/// <param name="context">The database context.</param>
public class PersonAddressRepository(DbContext context) : Repository<PersonAddressModel>(context), IPersonAddressRepository;