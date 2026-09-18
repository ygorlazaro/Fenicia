using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.Position.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position;

public class PositionRepository(DbContext context) : Repository<PositionModel>(context), IPositionRepository;