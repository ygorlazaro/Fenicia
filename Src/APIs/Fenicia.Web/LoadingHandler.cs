using Fenicia.Web.Services;

namespace Fenicia.Web;

public sealed class LoadingHandler(ILoadingService loadingService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        loadingService.Increment();
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        finally
        {
            loadingService.Decrement();
        }
    }
}
