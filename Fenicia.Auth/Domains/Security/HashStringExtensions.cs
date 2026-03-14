using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Security;

/// <summary>
///     Extension methods for secure string hashing.
/// </summary>
public static class HashStringExtensions
{
    /// <summary>
    ///     Hashes a string using BCrypt with a work factor of 12.
    /// </summary>
    /// <param name="original">The string to hash (e.g., password).</param>
    /// <returns>BCrypt hashed string.</returns>
    /// <exception cref="InvalidRequestException">Thrown when original is null or empty.</exception>
    public static string Hash(this string original)
    {
        if (string.IsNullOrEmpty(original))
        {
            throw new InvalidRequestException(ExceptionMessages.PasswordCannotBeNullOrEmpty);
        }

        var hashed = BCrypt.Net.BCrypt.HashPassword(original, BCrypt.Net.BCrypt.GenerateSalt(12));

        return hashed ?? throw new Exception(ExceptionMessages.ErrorHashingPassword);
    }
}