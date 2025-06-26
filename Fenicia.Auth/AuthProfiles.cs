using AutoMapper;

using Fenicia.Auth.Domains.Company.Logic;
using Fenicia.Auth.Domains.ForgotPassword.Data;
using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Auth.Domains.User.Data;

namespace Fenicia.Auth;

// ReSharper disable once UnusedType.Global
public class AuthProfiles : Profile
{
    public AuthProfiles()
    {
        CreateMap<CompanyRequest, CompanyModel>().ReverseMap();
        CreateMap<CompanyResponse, CompanyModel>().ReverseMap();
        CreateMap<CompanyUpdateRequest, CompanyModel>().ReverseMap();

        CreateMap<ModuleResponse, ModuleModel>().ReverseMap();

        CreateMap<OrderResponse, OrderModel>().ReverseMap();

        CreateMap<SubscriptionResponse, SubscriptionModel>().ReverseMap();

        CreateMap<UserResponse, UserModel>().ReverseMap();

        CreateMap<ForgotPasswordRequest, ForgotPasswordModel>().ReverseMap();
        CreateMap<ForgotPasswordResponse, ForgotPasswordModel>().ReverseMap();
        CreateMap<ForgotPasswordRequestReset, ForgotPasswordModel>().ReverseMap();
    }
}
