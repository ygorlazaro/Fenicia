using FluentValidation;

namespace Fenicia.Auth.Domains.Order.Data;

/// <summary>
/// Validator for the OrderRequest class that ensures all required data is present and valid
/// </summary>
public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    private readonly ILogger<OrderRequestValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderRequestValidator"/> class
    /// </summary>
    /// <param name="logger">The logger instance for validation operations</param>
    public OrderRequestValidator(ILogger<OrderRequestValidator> logger)
    {
        _logger = logger;

        try
        {
            RuleFor(x => x.Details)
                .NotEmpty().WithMessage("Details are required.");

            _logger.LogInformation("OrderRequestValidator rules configured successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring OrderRequestValidator rules");
            throw;
        }
    }
}
