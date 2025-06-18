namespace Fenicia.Module.Basic.Domains.State;

public interface IStateRepository
{
    Task<List<StateModel>> GetAllAsync();
}
