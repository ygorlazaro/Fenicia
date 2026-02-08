using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

public class OrderResponse
{
    public OrderResponse()
    {
        
    }

    public OrderResponse(OrderModel model)
    {
        this.Id = model.Id;
        this.SaleDate = model.SaleDate;
        this.Status = model.Status;
        this.TotalAmount = model.TotalAmount;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime SaleDate { get; set; }

    [Required]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [DataType(DataType.Currency)]
    public decimal TotalAmount { get; set; }
}