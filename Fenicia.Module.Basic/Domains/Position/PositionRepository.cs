using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.Position.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position;

/// <summary>
/// Repository implementation for managing position data.
/// </summary>
/// <param name="context">The database context.</param>
public class PositionRepository(DbContext context)
    : Repository<PositionModel>(context), IPositionRepository
{
}