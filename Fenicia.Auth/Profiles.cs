using AutoMapper;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;

namespace Fenicia.Auth;

// ReSharper disable once UnusedType.Global
public class Profiles: Profile
{
    public Profiles()
    {
        CreateMap<CompanyRequest, CompanyModel>().ReverseMap();
        CreateMap<CompanyResponse, CompanyModel>().ReverseMap();

        CreateMap<ModuleResponse, ModuleModel>().ReverseMap();
        
        CreateMap<OrderResponse, OrderModel>().ReverseMap();
        
        CreateMap<SubscriptionResponse, SubscriptionModel>().ReverseMap();
        
        CreateMap<UserResponse, UserModel>().ReverseMap();
    }
}