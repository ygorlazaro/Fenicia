using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.Enums.Basic;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class OrderDetailSummary
{
    [Parameter]
[EditorRequired]
public GetOrderByIdResponse Order { get; set; } = default!;

    private static string PaymentLabel(PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.Cash => "Dinheiro",

            PaymentMethod.CreditCard => "Cartão de Crédito",

            PaymentMethod.DebitCard => "Cartão de Débito",

            PaymentMethod.Pix => "PIX",

            PaymentMethod.Boleto => "Boleto",

            PaymentMethod.BankTransfer => "Transferência",

            _ => method.ToString()
        };
    }
}
