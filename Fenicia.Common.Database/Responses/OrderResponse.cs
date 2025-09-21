namespace Fenicia.Common.Database.Responses;

using System.ComponentModel.DataAnnotations;

using Enums;

using Models.Auth;

public class OrderResponse
{
    [Required]
    public Guid Id
    {
        get; set;
    }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime SaleDate
    {
        get; set;
    }

    [Required]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status
    {
        get; set;
    }

    [Required]
    [Range(minimum: 0, double.MaxValue)]
    [DataType(DataType.Currency)]
    public decimal TotalAmount
    {
        get; set;
    }

    public static OrderResponse Convert(OrderModel order)
    {
        return new OrderResponse { Id = order.Id, SaleDate = order.SaleDate, Status = order.Status, TotalAmount = order.TotalAmount };
    }
}
