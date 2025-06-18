namespace Fenicia.Module.Basic.Domains.State;

public interface IStateService
{
    Task<List<StateModel>> GetAllAsync();
}
