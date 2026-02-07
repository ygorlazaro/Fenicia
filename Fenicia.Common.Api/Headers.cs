using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Common.API;

public class Headers
{
    [FromHeader(Name = "x-company")]
    [Required]
    public Guid CompanyId { get; set; }
}