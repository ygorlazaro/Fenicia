using Fenicia.Common.DTOs.Auth.Register;

namespace Fenicia.Auth.Domains.Register.Interfaces;

/// <summary>
/// Defines the contract for a service responsible for handling user registration operations. This interface outlines the methods required to create new user accounts, ensuring that the registration process is standardized and can be implemented consistently across different parts of the application.
/// </summary>
public interface IRegisterService
{
    /// <summary>
    /// Creates a new user account based on the provided registration request. This method handles the necessary
    /// validation, user creation, and any additional setup required for a new user. It returns a response indicating the success or failure of the registration process.
    /// </summary>
    /// <param name="request">The registration request containing user details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the registration response.</returns>
    Task<RegisterResponse> CreateAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
