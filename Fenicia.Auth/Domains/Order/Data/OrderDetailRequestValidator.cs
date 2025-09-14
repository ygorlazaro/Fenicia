namespace Fenicia.Auth.Domains.Order.Data;

using Common.Database.Requests;

using FluentValidation;

public class OrderDetailRequestValidator : AbstractValidator<OrderDetailRequest>
{
    public OrderDetailRequestValidator(ILogger<OrderDetailRequestValidator> logger)
    {
        try
        {
            logger.LogInformation("Initializing OrderDetailRequest validation rules");

            RuleFor(x => x.ModuleId).NotEmpty().WithMessage("ModuleId is required.").WithErrorCode("INVALID_MODULE_ID").WithMessage("ModuleId must be greater than 0");

            logger.LogInformation("OrderDetailRequest validation rules initialized successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while initializing OrderDetailRequest validation rules");
            throw;
        }
    }
}
