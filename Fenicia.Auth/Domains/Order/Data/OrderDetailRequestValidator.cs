using Fenicia.Auth.Domains.OrderDetail.Data;
using FluentValidation;

namespace Fenicia.Auth.Domains.Order.Data;

/// <summary>
/// Validator for OrderDetailRequest to ensure data integrity and business rules compliance
/// </summary>
public class OrderDetailRequestValidator : AbstractValidator<OrderDetailRequest>
{
    private readonly ILogger<OrderDetailRequestValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the OrderDetailRequestValidator class and sets up validation rules
    /// </summary>
    /// <param name="logger">Logger instance for validation operations</param>
    public OrderDetailRequestValidator(ILogger<OrderDetailRequestValidator> logger)
    {
        _logger = logger;

        try
        {
            _logger.LogInformation("Initializing OrderDetailRequest validation rules");

            RuleFor(x => x.ModuleId)
                .NotEmpty()
                .WithMessage("ModuleId is required.")
                .WithErrorCode("INVALID_MODULE_ID")
                .WithMessage("ModuleId must be greater than 0");

            _logger.LogInformation("OrderDetailRequest validation rules initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while initializing OrderDetailRequest validation rules");
            throw;
        }
    }
}
