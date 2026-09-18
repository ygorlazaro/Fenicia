using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.DataSource;

public class GetAllProductForDataSourceResponse()
{
    public GetAllProductForDataSourceResponse(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}
