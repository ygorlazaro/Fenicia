namespace Fenicia.Auth.Domains.Order.Data;

using FluentValidation;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    private readonly ILogger<OrderRequestValidator> _logger;

    public OrderRequestValidator(ILogger<OrderRequestValidator> logger)
    {
        _logger = logger;

        try
        {
            RuleFor(x => x.Details).NotEmpty().WithMessage(errorMessage: "Details are required.");

            _logger.LogInformation(message: "OrderRequestValidator rules configured successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "Error configuring OrderRequestValidator rules");
            throw;
        }
    }
}
