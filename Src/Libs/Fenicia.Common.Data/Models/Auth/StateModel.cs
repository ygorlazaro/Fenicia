using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Models.Auth;

[Table("states", Schema = "auth")]
public sealed class StateModel : BaseModel
{
    [Required]
    [MaxLength(30)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(2)]
    public string Uf { get; init; } = string.Empty;

    public List<AddressModel> Addresses { get; init; } = [];

    public List<PersonModel> People { get; init; } = [];
}