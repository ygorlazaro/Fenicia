using Fenicia.Auth.Domains.Subscription.CreateCreditsForOrder;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Subscription;

public class CreateCreditsForOrderHandlerTests : IDisposable
{
    public CreateCreditsForOrderHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.handler = new CreateCreditsForOrderHandler(this.context);
    }

    public void Dispose()
    {
        this.context.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly CreateCreditsForOrderHandler handler;

    [Fact]
    public async Task Handle_WhenValidDetails_CreatesSubscriptionAndCreditsSuccessfully()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var module2Id = Guid.NewGuid();
        var detail1Id = Guid.NewGuid();
        var detail2Id = Guid.NewGuid();

        var details = new List<CreateCreditsForOrderDetailsQuery>
        {
            new(detail1Id, module1Id),
            new(detail2Id, module2Id)
        };

        var query = new CreateCreditsForOrderQuery(orderId, companyId, details);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        var subscription = await this.context.AuthSubscriptions.Include(subscriptionModel => subscriptionModel.Credits).FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.NotNull(subscription);
        Assert.Equal(companyId, subscription.CompanyId);
        Assert.Equal(orderId, subscription.OrderId);
        Assert.Equal(SubscriptionStatus.Active, subscription.Status);
        Assert.Equal(2, subscription.Credits.Count);
    }

    [Fact]
    public async Task Handle_CreatesCreditsWithCorrectProperties()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var detail1Id = Guid.NewGuid();

        var details = new List<CreateCreditsForOrderDetailsQuery>
        {
            new(detail1Id, module1Id)
        };

        var query = new CreateCreditsForOrderQuery(orderId, companyId, details);
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        var subscription = await this.context.AuthSubscriptions.Include(s => s.Credits)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.NotNull(subscription);

        var credit = subscription.Credits.First();
        Assert.Equal(module1Id, credit.ModuleId);
        Assert.Equal(detail1Id, credit.OrderDetailId);
        Assert.True(credit.IsActive);
        Assert.True(credit.StartDate >= beforeCall);
        Assert.True(credit.EndDate > credit.StartDate);
    }

    [Fact]
    public async Task Handle_SetsCorrectSubscriptionDates()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var detail1Id = Guid.NewGuid();

        var details = new List<CreateCreditsForOrderDetailsQuery>
        {
            new(detail1Id, module1Id)
        };

        var query = new CreateCreditsForOrderQuery(orderId, companyId, details);
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        var subscription = await this.context.AuthSubscriptions.FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.NotNull(subscription);
        Assert.True(subscription.StartDate >= beforeCall);
        Assert.True(subscription.EndDate > subscription.StartDate);
        Assert.Equal(subscription.StartDate.AddMonths(1).Month, subscription.EndDate.Month);
    }

    [Fact]
    public async Task Handle_WhenNoDetails_ThrowsArgumentException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var query = new CreateCreditsForOrderQuery(orderId, companyId, []);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(query, CancellationToken.None)
        );
        Assert.Equal("Order details cannot be empty", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenMultipleDetails_CreatesCreditForEach()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleIds = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList();
        var detailIds = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList();

        var details = moduleIds.Select((id, i) => new CreateCreditsForOrderDetailsQuery(detailIds[i], id)).ToList();
        var query = new CreateCreditsForOrderQuery(orderId, companyId, details);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        var subscription = await this.context.AuthSubscriptions.Include(s => s.Credits)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.NotNull(subscription);
        Assert.Equal(5, subscription.Credits.Count);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectResponseData()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var detail1Id = Guid.NewGuid();

        var details = new List<CreateCreditsForOrderDetailsQuery>
        {
            new(detail1Id, module1Id)
        };

        var query = new CreateCreditsForOrderQuery(orderId, companyId, details);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(companyId, result.CompanyId);
        Assert.Equal(orderId, result.OrderId);
        Assert.Equal(SubscriptionStatus.Active, result.Status);
    }

    [Fact]
    public async Task Handle_WhenSingleDetail_CreatesSubscriptionWithOneCredit()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var detail1Id = Guid.NewGuid();

        var details = new List<CreateCreditsForOrderDetailsQuery>
        {
            new(detail1Id, module1Id)
        };

        var query = new CreateCreditsForOrderQuery(orderId, companyId, details);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        var subscription = await this.context.AuthSubscriptions.Include(s => s.Credits)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.NotNull(subscription);
        Assert.Single(subscription.Credits);
        Assert.Equal(module1Id, subscription.Credits[0].ModuleId);
    }

    [Fact]
    public async Task Handle_VerifiesAllCreditsAreActive()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var moduleIds = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToList();
        var detailIds = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid()).ToList();

        var details = moduleIds.Select((id, i) => new CreateCreditsForOrderDetailsQuery(detailIds[i], id)).ToList();
        var query = new CreateCreditsForOrderQuery(orderId, companyId, details);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        var subscription = await this.context.AuthSubscriptions.Include(s => s.Credits)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.NotNull(subscription);
        Assert.True(subscription.Credits.All(c => c.IsActive));
    }

    [Fact]
    public async Task Handle_WhenCalledMultipleTimes_CreatesSeparateSubscriptions()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var detail1Id = Guid.NewGuid();

        var details = new List<CreateCreditsForOrderDetailsQuery>
        {
            new(detail1Id, module1Id)
        };

        var query1 = new CreateCreditsForOrderQuery(Guid.NewGuid(), companyId, details);
        var query2 = new CreateCreditsForOrderQuery(Guid.NewGuid(), companyId, details);

        // Act
        var result1 = await this.handler.Handle(query1, CancellationToken.None);
        var result2 = await this.handler.Handle(query2, CancellationToken.None);

        // Assert
        Assert.NotEqual(result1.Id, result2.Id);

        var subscriptions = await this.context.AuthSubscriptions.ToListAsync();
        Assert.Equal(2, subscriptions.Count);
    }

    [Fact]
    public async Task Handle_WhenDuplicateModuleIds_CreatesCreditForEach()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var module1Id = Guid.NewGuid();
        var detail1Id = Guid.NewGuid();
        var detail2Id = Guid.NewGuid();

        var details = new List<CreateCreditsForOrderDetailsQuery>
        {
            new(detail1Id, module1Id),
            new(detail2Id, module1Id) // Same module, different detail
        };

        var query = new CreateCreditsForOrderQuery(orderId, companyId, details);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        var subscription = await this.context.AuthSubscriptions.Include(s => s.Credits)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.NotNull(subscription);
        Assert.Equal(2, subscription.Credits.Count);
    }
}
