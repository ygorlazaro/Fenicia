using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Repositories.Interfaces;

public interface ISubscriptionRepository
{
    Task SaveSubscription(SubscriptionModel subscription);
}