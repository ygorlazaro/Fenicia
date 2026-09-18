namespace Fenicia.Common.DTOs.Basic.Customer;

public class GetCustomerInsightsQuery()
{
    public GetCustomerInsightsQuery(int days = 90, int topLimit = 10, int riskThresholdDays = 60)
        : this()
    {
        Days = days;
        TopLimit = topLimit;
        RiskThresholdDays = riskThresholdDays;
    }

    public int Days { get; set; }

    public int TopLimit { get; set; }

    public int RiskThresholdDays { get; set; }
}
