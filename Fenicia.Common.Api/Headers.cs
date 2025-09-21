namespace Fenicia.Common.API;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

public class Headers
{
    [FromHeader(Name = "x-company")]
    [Required]
    public Guid CompanyId
    {
        get; set;
    }
}
