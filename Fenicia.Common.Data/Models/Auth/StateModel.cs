using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Models.Auth;

[Table("states", Schema = "auth")]
public class StateModel : BaseModel
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(2)]
    public string Uf { get; set; } = null!;

    public virtual List<AddressModel> Addresses { get; set; } = null!;

    public virtual List<PersonModel> People { get; set; } = null!;
}
