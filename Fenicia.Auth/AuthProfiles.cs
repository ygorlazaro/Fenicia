using AutoMapper;
using Microsoft.Extensions.Logging;

using Fenicia.Auth.Domains.Company.Data;
using Fenicia.Auth.Domains.ForgotPassword.Data;
using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Auth.Domains.User.Data;

namespace Fenicia.Auth;

/// <summary>
/// Defines AutoMapper profile configurations for authentication-related domain models
/// </summary>
public class AuthProfiles : Profile
{
    /// <summary>
    /// Initializes a new instance of the AuthProfiles class and configures all the mapping profiles
    /// </summary>
    public AuthProfiles()
    {
        // Company mapping configurations
        CreateMap<CompanyRequest, CompanyModel>().ReverseMap();
        CreateMap<CompanyResponse, CompanyModel>().ReverseMap();
        CreateMap<CompanyUpdateRequest, CompanyModel>().ReverseMap();

        // Module mapping configurations
        CreateMap<ModuleResponse, ModuleModel>().ReverseMap();

        // Order mapping configurations
        CreateMap<OrderResponse, OrderModel>().ReverseMap();

        // Subscription mapping configurations
        CreateMap<SubscriptionResponse, SubscriptionModel>().ReverseMap();

        // User mapping configurations
        CreateMap<UserResponse, UserModel>().ReverseMap();

        // Forgot password mapping configurations
        CreateMap<ForgotPasswordRequest, ForgotPasswordModel>().ReverseMap();
        CreateMap<ForgotPasswordResponse, ForgotPasswordModel>().ReverseMap();
        CreateMap<ForgotPasswordRequestReset, ForgotPasswordModel>().ReverseMap();
    }
}
