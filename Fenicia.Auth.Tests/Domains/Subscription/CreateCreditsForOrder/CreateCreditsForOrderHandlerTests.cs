using Fenicia.Auth.Domains.Subscription.CreateCreditsForOrder;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Subscription.CreateCreditsForOrder;

[TestFixture]
public class CreateCreditsForOrderHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new CreateCreditsForOrderHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private AuthContext context = null!;
    private CreateCreditsForOrderHandler handler = null!;

    [Test]
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
        Assert.That(result, Is.Not.Null);

        var subscription = await this.context.Subscriptions.Include(subscriptionModel => subscriptionModel.Credits).FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.That(subscription, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(subscription!.CompanyId, Is.EqualTo(companyId), "CompanyId should match");
            Assert.That(subscription.OrderId, Is.EqualTo(orderId), "OrderId should match");
            Assert.That(subscription.Status, Is.EqualTo(SubscriptionStatus.Active), "Status should be Active");
            Assert.That(subscription.Credits, Has.Count.EqualTo(2), "Should have 2 credits");
        }
    }

    [Test]
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
        var subscription = await this.context.Subscriptions.Include(s => s.Credits)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.That(subscription, Is.Not.Null);

        var credit = subscription!.Credits.First();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(credit.ModuleId, Is.EqualTo(module1Id), "ModuleId should match");
            Assert.That(credit.OrderDetailId, Is.EqualTo(detail1Id), "OrderDetailId should match");
            Assert.That(credit.IsActive, Is.True, "Credit should be active");
            Assert.That(credit.StartDate, Is.GreaterThanOrEqualTo(beforeCall), "StartDate should be set");
            Assert.That(credit.EndDate, Is.GreaterThan(credit.StartDate), "EndDate should be after StartDate");
        }
    }

    [Test]
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
        var subscription = await this.context.Subscriptions.FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.That(subscription, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(subscription!.StartDate, Is.GreaterThanOrEqualTo(beforeCall), "StartDate should be set");
            Assert.That(subscription.EndDate, Is.GreaterThan(subscription.StartDate),
                "EndDate should be after StartDate");
            Assert.That(subscription.EndDate.Month, Is.EqualTo(subscription.StartDate.AddMonths(1).Month),
                "EndDate should be 1 month after StartDate");
        }
    }

    [Test]
    public void Handle_WhenNoDetails_ThrowsArgumentException()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var query = new CreateCreditsForOrderQuery(orderId, companyId, []);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(query, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Ocorreu um problema para adicionar créditos de assinatura"));
    }

    [Test]
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
        var subscription = await this.context.Subscriptions.Include(s => s.Credits)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.That(subscription, Is.Not.Null);
        Assert.That(subscription!.Credits, Has.Count.EqualTo(5), "Should create 5 credits");
    }

    [Test]
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty), "Id should be set");
            Assert.That(result.CompanyId, Is.EqualTo(companyId), "CompanyId should match");
            Assert.That(result.OrderId, Is.EqualTo(orderId), "OrderId should match");
            Assert.That(result.Status, Is.EqualTo(SubscriptionStatus.Active), "Status should be Active");
        }
    }

    [Test]
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
        var subscription = await this.context.Subscriptions.Include(s => s.Credits)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.That(subscription, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(subscription!.Credits, Has.Count.EqualTo(1), "Should have 1 credit");
            Assert.That(subscription.Credits[0].ModuleId, Is.EqualTo(module1Id), "ModuleId should match");
        }
    }

    [Test]
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
        var subscription = await this.context.Subscriptions.Include(s => s.Credits)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.That(subscription, Is.Not.Null);
        Assert.That(subscription!.Credits.All(c => c.IsActive), Is.True, "All credits should be active");
    }

    [Test]
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
        Assert.That(result1.Id, Is.Not.EqualTo(result2.Id), "Should create separate subscriptions");

        var subscriptions = await this.context.Subscriptions.ToListAsync();
        Assert.That(subscriptions, Has.Count.EqualTo(2), "Should have 2 subscriptions");
    }

    [Test]
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
        var subscription = await this.context.Subscriptions.Include(s => s.Credits)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        Assert.That(subscription, Is.Not.Null);
        Assert.That(subscription!.Credits, Has.Count.EqualTo(2), "Should create 2 credits even for same module");
    }
}