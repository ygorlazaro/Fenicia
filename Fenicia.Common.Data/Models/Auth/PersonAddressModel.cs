using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Auth;

[Table("person_addresses", Schema = "auth")]
public sealed class PersonAddressModel : BaseCompanyModel
{
    public Guid PersonId { get; init; }

    public Guid AddressId { get; init; }

    [ForeignKey(nameof(PersonId))]
    public PersonModel Person { get; init; } = default!;

    [ForeignKey(nameof(AddressId))]
    public AddressModel Address { get; init; } = default!;
}
