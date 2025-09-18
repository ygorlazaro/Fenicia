namespace Fenicia.Module.Basic.Domains.State;

using Fenicia.Common.Database.Models.Basic;

public interface IStateRepository
{
    Task<List<StateModel>> GetAllAsync();
}
