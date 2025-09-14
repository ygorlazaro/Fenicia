namespace Fenicia.Auth.Domains.Order.Data;

using Common.Database.Requests;

using FluentValidation;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator(ILogger<OrderRequestValidator> logger)
    {
        try
        {
            RuleFor(x => x.Details).NotEmpty().WithMessage("Details are required.");

            logger.LogInformation("OrderRequestValidator rules configured successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error configuring OrderRequestValidator rules");
            throw;
        }
    }
}
