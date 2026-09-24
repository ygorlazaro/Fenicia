using Fenicia.Common.DTOs.Basic.Position;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Position;

/// <summary>
/// Mapper class for mapping position data.
/// </summary>
public static class PositionMapper
{
    /// <summary>
    /// Maps a PositionModel to a PositionResponse.
    /// </summary>
    /// <param name="position">The position model.</param>
    /// <returns>The position response.</returns>
    public static PositionResponse MapToResponse(PositionModel position)
    {
        return new PositionResponse(position.Id, position.Name);
    }

    /// <summary>
    /// Maps a collection of PositionModel to a list of PositionResponse.
    /// </summary>
    /// <param name="positions">The position models.</param>
    /// <returns>A list of position responses.</returns>
    public static List<PositionResponse> MapToResponseList(IEnumerable<PositionModel> positions)
    {
        return [.. positions.Select(MapToResponse)];
    }
}