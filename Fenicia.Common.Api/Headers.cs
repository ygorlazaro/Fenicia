using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Common.API;

public class Headers
{
    // x-company header deprecated - use JWT company_id claim
    public Guid CompanyId { get; set; }
}
