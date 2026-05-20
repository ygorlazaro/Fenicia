using Fenicia.Auth.Domains.Notification.Queries;
using Fenicia.Auth.Domains.Notification.Responses;
using Fenicia.Common.Data.Contexts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification.Handlers;

public class GetNotificationByIdHandler(DefaultContext db) : IRequestHandler<GetNotificationByIdQuery, GetNotificationByIdResponse?>
{
    public async Task<GetNotificationByIdResponse?> Handle(GetNotificationByIdQuery query, CancellationToken ct)
    {
        var notification = await db.AuthNotifications
            .FirstOrDefaultAsync(n => n.Id == query.Id, ct);

        if (notification is null)
        {
            return null;
        }

        return new GetNotificationByIdResponse(
            notification.Id,
            notification.Title,
            notification.Description,
            notification.Date,
            notification.ImageUrl,
            notification.Read
        );
    }
}
