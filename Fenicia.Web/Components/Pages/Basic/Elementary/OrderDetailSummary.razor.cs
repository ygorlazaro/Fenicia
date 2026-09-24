using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.Enums.Basic;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class OrderDetailSummary
{
    [Parameter]
    [EditorRequired]
    public GetOrderByIdResponse Order { get; set; } = null!;

    private static string PaymentLabel(EnumPaymentMethod method)
    {
        return method switch
        {
            EnumPaymentMethod.Cash => "Dinheiro",

            EnumPaymentMethod.CreditCard => "Cartão de Crédito",

            EnumPaymentMethod.DebitCard => "Cartão de Débito",

            EnumPaymentMethod.Pix => "PIX",

            EnumPaymentMethod.Boleto => "Boleto",

            EnumPaymentMethod.BankTransfer => "Transferência",

            _ => method.ToString()
        };
    }
}
