using System.ComponentModel;

namespace Fenicia.Common.Enums.Basic;

public enum PaymentMethod
{
    [Description("Credit Card")]
    CreditCard = 0,

    [Description("Debit Card")]
    DebitCard = 1,

    [Description("PIX")]
    Pix = 2,

    [Description("Boleto")]
    Boleto = 3,

    [Description("Cash")]
    Cash = 4,

    [Description("Bank Transfer")]
    BankTransfer = 5
}
