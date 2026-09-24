using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Address.Interfaces;

/// <summary>
/// Defines the contract for a repository that manages address-related data operations.
/// </summary>
public interface IAddressRepository : IRepository<AddressModel>;