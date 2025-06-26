namespace Fenicia.Auth.Domains.Order.Data;

using FluentValidation;

using OrderDetail.Data;

/// <summary>
///     Validator for OrderDetailRequest to ensure data integrity and business rules compliance
/// </summary>
public class OrderDetailRequestValidator : AbstractValidator<OrderDetailRequest>
{
    private readonly ILogger<OrderDetailRequestValidator> _logger;

    /// <summary>
    ///     Initializes a new instance of the OrderDetailRequestValidator class and sets up validation rules
    /// </summary>
    /// <param name="logger">Logger instance for validation operations</param>
    public OrderDetailRequestValidator(ILogger<OrderDetailRequestValidator> logger)
    {
        _logger = logger;

        try
        {
            _logger.LogInformation(message: "Initializing OrderDetailRequest validation rules");

            RuleFor(x => x.ModuleId).NotEmpty().WithMessage(errorMessage: "ModuleId is required.").WithErrorCode(errorCode: "INVALID_MODULE_ID").WithMessage(errorMessage: "ModuleId must be greater than 0");

            _logger.LogInformation(message: "OrderDetailRequest validation rules initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "Error occurred while initializing OrderDetailRequest validation rules");
            throw;
        }
    }
}
