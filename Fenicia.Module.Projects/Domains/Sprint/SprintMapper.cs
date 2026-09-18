using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.Sprint;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Projects.Domains.Sprint;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SprintMapper
{
    public partial GetAllSprintResponse MapToGetAllSprintResponse(SprintModel sprint);

    public partial GetSprintByIdResponse MapToGetSprintByIdResponse(SprintModel sprint);

    public partial AddSprintResponse MapToAddSprintResponse(SprintModel sprint);

    public partial UpdateSprintResponse MapToUpdateSprintResponse(SprintModel sprint);
}
