using Fenicia.Common.DTOs.Auth.Register;

namespace Fenicia.Auth.Domains.Register.Interfaces;

public interface IRegisterService
{
    Task<RegisterResponse> CreateAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}