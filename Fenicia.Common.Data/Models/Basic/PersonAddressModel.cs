using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.Basic;

[Table("person_addresses", Schema = "basic")]
public sealed class PersonAddressModel : BaseCompanyModel
{
    public Guid PersonId { get; init; }

    public Guid AddressId { get; init; }

    [ForeignKey(nameof(PersonId))]
    public PersonModel Person { get; init; } = default!;

    [ForeignKey(nameof(AddressId))]
    public AddressModel Address { get; init; } = default!;
}