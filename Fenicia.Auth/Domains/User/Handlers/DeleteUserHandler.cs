using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Common.Data.Contexts;

using MediatR;

namespace Fenicia.Auth.Domains.User.Handlers;

public class DeleteUserHandler(DefaultContext db) : IRequestHandler<DeleteUserCommand>
{
    public virtual async Task Handle(DeleteUserCommand command, CancellationToken ct)
    {
        var user = await db.AuthUsers.FirstByIdAsync(command.UserId, ct);

        user.Deleted = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }
}
