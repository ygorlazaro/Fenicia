using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Models.Basic;

[Table("orders")]
public sealed class OrderModel : BaseModel
{
    public OrderModel(OrderRequest request)
    {
        this.CustomerId = request.CustomerId;
        this.Details = [.. request.Details.Select(d => new OrderDetailModel(d))];
        this.SaleDate = request.SaleDate;
        this.Status = request.Status;
        this.UserId = request.UserId;
        this.Id = request.Id;
    }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime SaleDate { get; set; }

    [Required]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; }

    [JsonIgnore]
    public List<OrderDetailModel> Details { get; set; }

    [ForeignKey(nameof(CustomerId))]
    [JsonIgnore]
    public CustomerModel Customer { get; set; } = null!;
}
