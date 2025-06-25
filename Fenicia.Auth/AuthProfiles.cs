using AutoMapper;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.User;

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
