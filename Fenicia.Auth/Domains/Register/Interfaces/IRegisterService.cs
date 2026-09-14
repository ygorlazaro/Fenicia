using Fenicia.Common.DTOs.Auth.Register;

namespace Fenicia.Auth.Domains.Register.Interfaces;

public interface IRegisterService
{
    Task<RegisterResponse> CreateAsync(RegisterCommand request, CancellationToken cancellationToken = default);
}