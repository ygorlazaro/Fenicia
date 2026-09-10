namespace Fenicia.Web.Components.Pages.Basic.Models;

public class OrderFormCreateOrderResponseDto
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
}
