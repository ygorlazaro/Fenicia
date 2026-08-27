namespace Fenicia.Module.Basic.Domains.Dashboard.DTOs.Responses;

public record RevenueVsCostResponse(

    string Period,

    DateTime Date,

    decimal Revenue,

    decimal Cost,

    decimal Profit);