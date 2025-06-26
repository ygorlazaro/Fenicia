namespace Fenicia.Auth.Domains.SubscriptionCredit.Logic;

using Common.Enums;

using Contexts;

using Microsoft.EntityFrameworkCore;

/// <summary>
///     Repository for managing subscription credits and their associated modules
/// </summary>
public class SubscriptionCreditRepository(AuthContext authContext) : ISubscriptionCreditRepository
{
    private readonly ILogger<SubscriptionCreditRepository> _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SubscriptionCreditRepository>();

    /// <summary>
    ///     Retrieves a list of valid module types for the given subscriptions
    /// </summary>
    /// <param name="subscriptions">List of subscription IDs to check</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>A list of valid ModuleType values</returns>
    public async Task<List<ModuleType>> GetValidModulesTypesAsync(List<Guid> subscriptions, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(message: "Starting to retrieve valid module types for {SubscriptionCount} subscriptions", subscriptions.Count);

            var now = DateTime.UtcNow;

            var query = from credit in authContext.SubscriptionCredits join module in authContext.Modules on credit.ModuleId equals module.Id where credit.IsActive && subscriptions.Contains(credit.SubscriptionId) && now >= credit.StartDate && now <= credit.EndDate orderby module.Id select module.Type;

            var result = await query.Distinct().ToListAsync(cancellationToken);

            _logger.LogInformation(message: "Successfully retrieved {ModuleCount} valid module types", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "Error retrieving valid module types for subscriptions");
            throw;
        }
    }
}
