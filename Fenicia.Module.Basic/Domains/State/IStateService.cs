namespace Fenicia.Module.Basic.Domains.State;

using Fenicia.Common.Database.Models.Basic;

public interface IStateService
{
    Task<List<StateModel>> GetAllAsync();
}
