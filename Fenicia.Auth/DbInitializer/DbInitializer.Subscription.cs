using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Auth.DbInitializer;

internal static partial class DbInitializer
{
    private static void SeedAuthOrders(DefaultContext context)
    {
        if (context.AuthOrders.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var users = context.AuthUsers.ToList();

        if (!users.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 40; i++)
        {
            var saleDate = now.AddDays(-(random.NextDouble() * 365));
            var orderNumber = $"AUTH-{saleDate.Year}-{i:D5}";

            var authOrder = new OrderModel()
            {
                CompanyId = companyId,
                OrderNumber = orderNumber[..Math.Min(orderNumber.Length, 20)],
                UserId = users[(i - 1) % users.Count].Id,
                TotalAmount = Math.Round((decimal)((random.NextDouble() * 10000) + 500), 2),
                DiscountAmount = Math.Round((decimal)(random.NextDouble() * 500), 2),
                TotalQuantity = random.Next(1, 11),
                SaleDate = saleDate,
                Status = (OrderStatus)(i % 3),
                PaymentMethod = (PaymentMethod)(i % 6),
                Notes = i % 5 == 0 ? $"Pedido módulo {i}" : null,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.AuthOrders.Add(authOrder);
        }
    }

    private static void SeedAuthOrderDetails(DefaultContext context)
    {
        if (context.AuthOrderDetails.Any())
        {
            return;
        }

        var orders = context.AuthOrders.ToList();
        var modules = context.AuthModules.ToList();

        if (!orders.Any() || !modules.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 100; i++)
        {
            var price = Math.Round((decimal)((random.NextDouble() * 500) + 50), 2);
            var discountAmount = Math.Round((decimal)(random.NextDouble() * 50), 2);
            var subtotal = Math.Round((decimal)((random.NextDouble() * 500) + 50), 2);

            var authOrderDetail = new OrderDetailModel
            {
                OrderId = orders[(i - 1) % orders.Count].Id,
                ModuleId = modules[(i - 1) % modules.Count].Id,
                Price = price,
                DiscountAmount = discountAmount,
                Subtotal = subtotal,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.AuthOrderDetails.Add(authOrderDetail);
        }
    }

    private static void SeedSubscriptions(DefaultContext context)
    {
        if (context.AuthSubscriptions.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var orders = context.AuthOrders.ToList();

        if (!orders.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 30; i++)
        {
            var startDate = now.AddDays(-(random.NextDouble() * 365));
            var endDate = now.AddDays(random.NextDouble() * 365);

            var subscription = new SubscriptionModel
            {
                Status = i % 4 == 0 ? SubscriptionStatus.Inactive : SubscriptionStatus.Active,
                CompanyId = companyId,
                StartDate = startDate,
                EndDate = endDate,
                OrderId = orders[(i - 1) % orders.Count].Id,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.AuthSubscriptions.Add(subscription);
        }
    }

    private static void SeedSubscriptionCredits(DefaultContext context)
    {
        if (context.AuthSubscriptionCredits.Any())
        {
            return;
        }

        var subscriptions = context.AuthSubscriptions.ToList();
        var modules = context.AuthModules.ToList();
        var orderDetails = context.AuthOrderDetails.ToList();

        if (!subscriptions.Any() || !modules.Any() || !orderDetails.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 60; i++)
        {
            var startDate = now.AddDays(-(random.NextDouble() * 365));
            var endDate = now.AddDays(random.NextDouble() * 365);

            var subscriptionCredit = new SubscriptionCreditModel
            {
                SubscriptionId = subscriptions[(i - 1) % subscriptions.Count].Id,
                ModuleId = modules[(i - 1) % modules.Count].Id,
                IsActive = i % 2 == 0,
                StartDate = startDate,
                EndDate = endDate,
                OrderDetailId = orderDetails[(i - 1) % orderDetails.Count].Id,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.AuthSubscriptionCredits.Add(subscriptionCredit);
        }
    }

    private static void SeedConfigurations(DefaultContext context)
    {
        if (context.AuthConfigurations.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var users = context.AuthUsers.ToList();

        if (!users.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 100; i++)
        {
            var configType = (ConfigType)(i % 2);
            var value = configType == ConfigType.Language ? "pt-BR" : "America/Sao_Paulo";

            var configuration = new ConfigurationModel
            {
                CompanyId = companyId,
                ConfigType = configType,
                Value = value,
                UserId = users[(i - 1) % users.Count].Id,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.AuthConfigurations.Add(configuration);
        }
    }

    private static void SeedNotifications(DefaultContext context)
    {
        if (context.AuthNotifications.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 80; i++)
        {
            var date = now.AddDays(-(random.NextDouble() * 365));

            var notification = new NotificationModel
            {
                CompanyId = companyId,
                Title = $"Notificação {i}",
                Description = $"Descrição da notificação {i}",
                Date = date,
                ImageUrl = i % 3 == 0 ? $"https://picsum.photos/seed/notif-{i}/100/100" : null,
                Read = i % 3 == 0,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.AuthNotifications.Add(notification);
        }
    }

    private static void SeedUploads(DefaultContext context)
    {
        if (context.AuthUploads.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 100; i++)
        {
            var upload = new UploadModel
            {
                OriginalFileName = $"arquivo-{i}.jpg",
                StoredFileName = $"stored-{i}.jpg",
                ContentType = "image/jpeg",
                SizeBytes = (long)((random.NextDouble() * 500000) + 50000),
                Url = $"https://fenicia.s3.amazonaws.com/uploads/stored-{i}.jpg",
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.AuthUploads.Add(upload);
        }
    }

    private static void SeedProfiles(DefaultContext context)
    {
        if (context.AuthProfiles.Any())
        {
            return;
        }

        var users = context.AuthUsers.ToList();
        var uploads = context.AuthUploads.ToList();

        if (!users.Any() || !uploads.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 0; i < users.Count; i++)
        {
            var birthDate = new DateTime(1990, 1, 1).AddDays(i % 12000);

            var profile = new ProfileModel
            {
                UserId = users[i].Id,
                UserName = $"usuario{i + 1}",
                Bio = i % 3 == 0 ? $"Bio do usuário {i + 1} apaixonado por tecnologia e inovação." : null,
                UploadId = uploads[i % uploads.Count].Id,
                Website = i % 4 == 0 ? $"https://usuario{i + 1}.dev" : null,
                Location = i % 2 == 0 ? "São Paulo, SP" : "Rio de Janeiro, RJ",
                Phone = $"({(i % 90) + 10}) 9{(i % 90000000) + 10000000:D8}",
                BirthDate = birthDate,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.AuthProfiles.Add(profile);
        }
    }
}
