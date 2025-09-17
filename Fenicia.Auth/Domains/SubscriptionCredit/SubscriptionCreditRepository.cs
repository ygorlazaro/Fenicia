namespace Fenicia.Auth.Domains.SubscriptionCredit;

using Common.Enums;

using Common.Database.Contexts;

using Microsoft.EntityFrameworkCore;

public class SubscriptionCreditRepository : ISubscriptionCreditRepository
{
    private readonly ILogger<SubscriptionCreditRepository> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SubscriptionCreditRepository>();
    private readonly AuthContext authContext;

    public SubscriptionCreditRepository(AuthContext authContext)
    {
        this.authContext = authContext;
    }

    public async Task<List<ModuleType>> GetValidModulesTypesAsync(List<Guid> subscriptions, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Starting to retrieve valid module types for {SubscriptionCount} subscriptions", subscriptions.Count);

            var now = DateTime.UtcNow;

            var query = from credit in this.authContext.SubscriptionCredits join module in this.authContext.Modules on credit.ModuleId equals module.Id where credit.IsActive && subscriptions.Contains(credit.SubscriptionId) && now >= credit.StartDate && now <= credit.EndDate orderby module.Id select module.Type;

            var result = await query.Distinct().ToListAsync(cancellationToken);

            this.logger.LogInformation("Successfully retrieved {ModuleCount} valid module types", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving valid module types for subscriptions");
            throw;
        }
    }
}
