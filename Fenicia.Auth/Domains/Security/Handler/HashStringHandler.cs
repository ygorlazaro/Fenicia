using Fenicia.Auth.Domains.Security.Command;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using MediatR;

namespace Fenicia.Auth.Domains.Security.Handler
{
    public class HashStringHandler : IRequestHandler<HashStringCommand, string>
    {
        public Task<string> Handle(HashStringCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Original))
            {
                throw new InvalidRequestException(ExceptionMessages.PasswordCannotBeNullOrEmpty);
            }

            var hashed = BCrypt.Net.BCrypt.HashPassword(request.Original, BCrypt.Net.BCrypt.GenerateSalt(12));

            return Task.FromResult(hashed ?? throw new Exception(ExceptionMessages.ErrorHashingPassword));
        }
    }
}

namespace Fenicia.Auth.Domains.Security
{
    public static class HashStringExtensions
    {
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
}
