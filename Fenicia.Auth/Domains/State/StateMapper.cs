using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.State;

namespace Fenicia.Auth.Domains.State;

/// <summary>
/// Mapper class for mapping between StateModel and StateResponse/StateRequest.
/// </summary>
public static class StateMapper
{
    /// <summary>
    /// Maps a StateModel to a StateResponse.
    /// </summary>
    /// <param name="state">The state model.</param>
    /// <returns>The state response.</returns>
    public static StateResponse MapToStateResponse(StateModel state)
    {
        return new StateResponse(state.Id, state.Name, state.Uf);
    }

    /// <summary>
    /// Maps a StateRequest to a StateModel.
    /// </summary>
    /// <param name="request">The state request.</param>
    /// <returns>The state model.</returns>
    public static StateModel MapToStateModel(StateRequest request)
    {
        return new StateModel
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            Uf = string.Empty
        };
    }
}
