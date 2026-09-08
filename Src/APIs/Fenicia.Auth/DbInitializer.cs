using System.Linq;
using Fenicia.Auth.Domains.Security;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Enums.SocialNetwork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AuthOrderDetailModel = Fenicia.Common.Data.Models.Auth.OrderDetailModel;
using AuthOrderModel = Fenicia.Common.Data.Models.Auth.OrderModel;
using ForgotPasswordModel = Fenicia.Common.Data.Models.Auth.ForgotPasswordModel;
using OrderDetailModel = Fenicia.Common.Data.Models.Basic.OrderDetailModel;
using OrderModel = Fenicia.Common.Data.Models.Basic.OrderModel;
using ProjectAttachmentModel = Fenicia.Common.Data.Models.Project.AttachmentModel;
using SocialAttachmentModel = Fenicia.Common.Data.Models.SocialNetwork.AttachmentModel;
using SocialCommentModel = Fenicia.Common.Data.Models.SocialNetwork.CommentModel;
using SubscriptionCreditModel = Fenicia.Common.Data.Models.Auth.SubscriptionCreditModel;

namespace Fenicia.Auth;

public static class DbInitializer
{
    private static SecurityService SecurityService { get; set; } = null!;

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        SecurityService = scope.ServiceProvider.GetRequiredService<SecurityService>();

        await context.Database.MigrateAsync();
        await CreateMissingIndexesAsync(context);
        Seed(context);
    }

    private static async Task CreateMissingIndexesAsync(DefaultContext context)
    {
#pragma warning disable CA2100
        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var commands = new[]
        {
            @"CREATE INDEX IF NOT EXISTS ix_product_categories_company_id ON basic.product_categories (company_id);",
            @"CREATE INDEX IF NOT EXISTS ix_positions_company_id ON basic.positions (company_id);",
            @"CREATE INDEX IF NOT EXISTS ix_suppliers_company_id ON basic.suppliers (company_id);",
            @"CREATE INDEX IF NOT EXISTS ix_employees_company_id ON basic.employees (company_id);",
            @"CREATE INDEX IF NOT EXISTS ix_customers_company_id ON basic.customers (company_id);",
            @"CREATE INDEX IF NOT EXISTS ix_products_company_id ON basic.products (company_id);",
            @"CREATE INDEX IF NOT EXISTS ix_stock_movements_company_id ON basic.stock_movements (company_id);"
        };

        foreach (var command in commands)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = command;
            await cmd.ExecuteNonQueryAsync();
        }
#pragma warning restore CA2100
    }

    private static void Seed(DefaultContext context)
    {
        SeedRoles(context);
        context.SaveChanges();
        SeedStates(context);
        context.SaveChanges();
        SeedModules(context);
        context.SaveChanges();
        SeedCompanies(context);
        context.SaveChanges();
        SeedAddresses(context);
        context.SaveChanges();
        SeedCompanyAddress(context);
        context.SaveChanges();
        SeedPeople(context);
        context.SaveChanges();
        SeedPosition(context);
        context.SaveChanges();
        SeedSuppliers(context);
        context.SaveChanges();
        SeedCustomers(context);
        context.SaveChanges();
        SeedEmployees(context);
        context.SaveChanges();
        SeedProductCategories(context);
        context.SaveChanges();
        SeedProducts(context);
        context.SaveChanges();
        SeedOrders(context);
        context.SaveChanges();
        SeedOrderDetails(context);
        context.SaveChanges();
        SeedStockMovements(context);
        context.SaveChanges();
        SeedPersonAddresses(context);
        context.SaveChanges();
        SeedUser(context);
        context.SaveChanges();
        SeedAuthOrders(context);
        context.SaveChanges();
        SeedAuthOrderDetails(context);
        context.SaveChanges();
        SeedSubscriptions(context);
        context.SaveChanges();
        SeedSubscriptionCredits(context);
        context.SaveChanges();
        SeedConfigurations(context);
        context.SaveChanges();
        SeedNotifications(context);
        context.SaveChanges();
        SeedForgottenPasswords(context);
        context.SaveChanges();
        SeedUploads(context);
        context.SaveChanges();
        SeedProfiles(context);
        context.SaveChanges();
        SeedFeeds(context);
        context.SaveChanges();
        SeedSocialComments(context);
        context.SaveChanges();
        SeedSocialLikes(context);
        context.SaveChanges();
        SeedSocialShares(context);
        context.SaveChanges();
        SeedSocialBlocks(context);
        context.SaveChanges();
        SeedSocialReports(context);
        context.SaveChanges();
        SeedSocialAttachments(context);
        context.SaveChanges();
        SeedProjects(context);
        context.SaveChanges();
        SeedProjectStatuses(context);
        context.SaveChanges();
        SeedSprints(context);
        context.SaveChanges();
        SeedTasks(context);
        context.SaveChanges();
        SeedProjectSubtasks(context);
        context.SaveChanges();
        SeedTaskAssignees(context);
        context.SaveChanges();
        SeedProjectComments(context);
        context.SaveChanges();
        SeedProjectAttachments(context);
        context.SaveChanges();
        SeedTeams(context);
        context.SaveChanges();
        SeedTeamUsers(context);
        context.SaveChanges();
    }

    private static void SeedProductCategories(DefaultContext context)
    {
        if (!context.BasicProductCategories.Any())
        {
            var categoryNames = new[]
            {
                "Eletrônicos", "Eletrodomésticos", "Móveis", "Informática", "Periféricos",
                "Celulares", "Acessórios", "Áudio e Vídeo", "Games", "Câmeras",
                "Smart Home", "Wearables", "Papelaria", "Escritório", "Material Escolar",
                "Limpeza", "Higiene Pessoal", "Alimentos", "Bebidas", "Congelados",
                "Padaria", "Hortifruti", "Carnes", "Laticínios", "Bebidas Alcoólicas",
                "Pet Shop", "Farmacêutico", "Beleza", "Perfumaria", "Automotivo",
                "Ferramentas", "Construção", "Jardim", "Esportes", "Fitness",
                "Camping", "Brinquedos", "Bebês", "Moda Infantil", "Moda Feminina",
                "Moda Masculina", "Calçados", "Bolças e Mochilas", "Relógios", "Joias",
                "Óculos", "Livros", "Revistas", "Música", "Filmes"
            };

            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();

            foreach (var categoryName in categoryNames)
            {
                var category = new ProductCategoryModel
                {
                    Name = categoryName,
                    CompanyId = companyId
                };

                context.BasicProductCategories.Add(category);
            }
        }
    }

    private static void SeedProducts(DefaultContext context)
    {
        if (!context.BasicProducts.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var categories = context.BasicProductCategories.ToList();
            var suppliers = context.BasicSuppliers.ToList();

            if (!categories.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 100; i++)
            {
                var category = categories[(i - 1) % categories.Count];

                string name;
                switch (i % 10)
                {
                    case 0: name = $"Produto {i} — Smartphone"; break;
                    case 1: name = $"Produto {i} — Notebook"; break;
                    case 2: name = $"Produto {i} — Monitor"; break;
                    case 3: name = $"Produto {i} — Teclado"; break;
                    case 4: name = $"Produto {i} — Mouse"; break;
                    case 5: name = $"Produto {i} — Cadeira"; break;
                    case 6: name = $"Produto {i} — Mesa"; break;
                    case 7: name = $"Produto {i} — Impressora"; break;
                    case 8: name = $"Produto {i} — Headset"; break;
                    default: name = $"Produto {i} — Webcam"; break;
                }

                var costPrice = Math.Round((decimal)((random.NextDouble() * 500) + 50), 2);
                var salesPrice = Math.Round((decimal)((random.NextDouble() * 1000) + 100), 2);
                var quantity = random.NextDouble() * 500;
                var weight = Math.Round((decimal)((random.NextDouble() * 10) + 0.1), 2);
                var dim1 = (random.NextDouble() * 100) + 10;
                var dim2 = (random.NextDouble() * 100) + 10;
                var dim3 = (random.NextDouble() * 100) + 10;

                var product = new ProductModel
                {
                    CompanyId = companyId,
                    Name = name[..Math.Min(name.Length, 50)],
                    SKU = $"SKU-{i:D5}",
                    Barcode = ((i % 9000000000000) + 1000000000000).ToString("D13"),
                    Description = $"Desc do produto {i}".Length <= 1000 ? $"Desc do produto {i}" : $"Desc do produto {i}"[..1000],
                    CostPrice = costPrice,
                    SalesPrice = salesPrice,
                    Quantity = quantity,
                    MinStockLevel = 10 + (i % 40),
                    MaxStockLevel = 200 + (i % 500),
                    ImageUrl = i % 3 == 0 ? $"https://picsum.photos/seed/prod-{i}/200/200" : null,
                    Weight = weight,
                    Dimensions = $"{dim1:F1}x{dim2:F1}x{dim3:F1} cm",
                    UnitOfMeasure = i % 2 == 0 ? "UN" : "CX",
                    CategoryId = category.Id,
                    SupplierId = i % 3 == 0 && suppliers.Any() ? suppliers[random.Next(suppliers.Count)].Id : null,
                    IsActive = true,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.BasicProducts.Add(product);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedPosition(DefaultContext context)
    {
        if (!context.BasicPositions.Any())
        {
            var positionNames = new[]
            {
                "CEO",
                "CTO",
                "CFO",
                "Diretor de Operações",
                "Gerente de TI",
                "Gerente de RH",
                "Gerente de Vendas",
                "Gerente de Marketing",
                "Gerente de Projetos",
                "Gerente Financeiro",
                "Coordenador de Desenvolvimento",
                "Coordenador de Suporte",
                "Coordenador de Qualidade",
                "Analista de Sistemas Sênior",
                "Analista de Sistemas Pleno",
                "Analista de Sistemas Júnior",
                "Desenvolvedor Full Stack Sênior",
                "Desenvolvedor Full Stack Pleno",
                "Desenvolvedor Full Stack Júnior",
                "Desenvolvedor Backend Sênior",
                "Desenvolvedor Backend Pleno",
                "Desenvolvedor Backend Júnior",
                "Desenvolvedor Frontend Sênior",
                "Desenvolvedor Frontend Pleno",
                "Desenvolvedor Frontend Júnior",
                "Desenvolvedor Mobile Sênior",
                "Desenvolvedor Mobile Pleno",
                "Desenvolvedor Mobile Júnior",
                "Engenheiro de Dados",
                "Cientista de Dados",
                "Analista de Dados",
                "DevOps Engineer Sênior",
                "DevOps Engineer Pleno",
                "DevOps Engineer Júnior",
                "QA Engineer Sênior",
                "QA Engineer Pleno",
                "QA Engineer Júnior",
                "Product Owner",
                "Scrum Master",
                "Tech Lead",
                "Arquiteto de Software",
                "Designer UX Sênior",
                "Designer UX Pleno",
                "Designer UX Júnior",
                "Designer UI Sênior",
                "Designer UI Pleno",
                "Designer UI Júnior",
                "Analista de Marketing Digital",
                "Especialista em SEO",
                "Social Media Manager",
                "Redator Publicitário",
                "Analista de Vendas",
                "Executivo de Contas",
                "Representante Comercial",
                "Analista de Suporte Técnico",
                "Especialista de Suporte N2",
                "Especialista de Suporte N3",
                "Analista de Infraestrutura",
                "Administrador de Redes",
                "Analista de Segurança da Informação",
                "Especialista em Cloud",
                "SRE Engineer",
                "Analista de QA Manual",
                "Analista de QA Automatizado",
                "Product Manager",
                "Business Analyst",
                "Analista de BI",
                "Analista de Compliance",
                "Advogado Corporativo",
                "Assistente Jurídico",
                "Analista de Recrutamento e Seleção",
                "Especialista em Treinamento",
                "Analista de Folha de Pagamento",
                "Assistente Administrativo",
                "Recepcionista",
                "Auxiliar de Escritório",
                "Analista de Controladoria",
                "Analista Fiscal",
                "Analista Contábil",
                "Tesoureiro",
                "Analista de Crédito",
                "Analista de Cobrança",
                "Analista de Logística",
                "Coordenador de Logística",
                "Analista de Compras",
                "Comprador Pleno",
                "Comprador Sênior",
                "Analista de Importação",
                "Analista de Exportação",
                "Analista de Qualidade",
                "Inspetor de Qualidade",
                "Analista de Meio Ambiente",
                "Técnico de Segurança do Trabalho",
                "Analista de Comunicação Interna",
                "Assessor de Imprensa",
                "Analista de Relações Públicas",
                "Designer Gráfico",
                "Editor de Vídeo",
                "Fotógrafo",
                "Videomaker"
            };

            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();

            foreach (var positionName in positionNames)
            {
                var position = new PositionModel
                {
                    Name = positionName,
                    CompanyId = companyId
                };

                context.BasicPositions.Add(position);
            }

            context.SaveChanges();
        }
    }

    private static void SeedAddresses(DefaultContext context)
    {
        if (!context.AuthAddresses.Any())
        {
            var cities = new[]
            {
                "São Paulo", "Rio de Janeiro", "Belo Horizonte", "Curitiba", "Porto Alegre",
                "Salvador", "Recife", "Fortaleza", "Brasília", "Manaus",
                "Belém", "Goiânia", "Campinas", "São Luís", "Maceió",
                "Natal", "Teresina", "Cuiabá", "Campo Grande", "Florianópolis",
                "João Pessoa", "Aracaju", "Palmas", "Macapá", "Boa Vista",
                "Porto Velho", "Rio Branco", "Vitória", "São Bernardo do Campo", "Santo André",
                "Osasco", "Sorocaba", "Ribeirão Preto", "Uberlândia", "Contagem",
                "Juiz de Fora", "Joinville", "Londrina", "Caxias do Sul", "Maringá"
            };

            var states = context.AuthStates.ToList();
            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 100; i++)
            {
                int cityIndex;
                if (i <= 40)
                {
                    cityIndex = i - 1;
                }
                else
                {
                    cityIndex = i % 40;
                }

                var state = states[random.Next(states.Count)];

                var address = new AddressModel
                {
                    Street = $"Rua {(i % 50) + 1}",
                    Number = ((i % 1000) + 1).ToString(),
                    Complement = i % 3 == 0 ? $"Apto {(i % 20) + 1}" : null,
                    Neighborhood = $"Bairro {(i % 20) + 1}",
                    ZipCode = ((i % 90000000) + 10000000).ToString("D8"),
                    StateId = state.Id,
                    City = cities[cityIndex],
                    Country = "Brasil",
                    AddressType = (AddressType)((i % 3) + 1),
                    Latitude = -23.55 + ((random.NextDouble() - 0.5) * 0.5),
                    Longitude = -46.63 + ((random.NextDouble() - 0.5) * 0.5),
                    IsDefault = i % 10 == 0,
                    Observation = i % 5 == 0 ? $"Observação endereço {i}" : null,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.AuthAddresses.Add(address);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedCompanyAddress(DefaultContext context)
    {
        var company = context.AuthCompanies.FirstOrDefault(x => x.AddressId == null);
        if (company != null)
        {
            var address = context.AuthAddresses.FirstOrDefault();
            if (address != null)
            {
                company.AddressId = address.Id;
            }
        }
    }

    private static void SeedPeople(DefaultContext context)
    {
        if (!context.BasicPeople.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var states = context.AuthStates.ToList();
            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 200; i++)
            {
                string name;
                switch (i % 10)
                {
                    case 0: name = $"Pessoa {i} Silva"; break;
                    case 1: name = $"Pessoa {i} Santos"; break;
                    case 2: name = $"Pessoa {i} Oliveira"; break;
                    case 3: name = $"Pessoa {i} Souza"; break;
                    case 4: name = $"Pessoa {i} Lima"; break;
                    case 5: name = $"Pessoa {i} Pereira"; break;
                    case 6: name = $"Pessoa {i} Costa"; break;
                    case 7: name = $"Pessoa {i} Rodrigues"; break;
                    case 8: name = $"Pessoa {i} Almeida"; break;
                    default: name = $"Pessoa {i} Ferreira"; break;
                }

                var document = i % 3 == 0 ? ((i % 90000000000) + 10000000000).ToString("D11") : null;
                var email = $"pessoa{i}@email.com";
                var phone = $"({(i % 90) + 10}) 9{(i % 90000000) + 10000000:D8}";
                var dateOfBirth = new DateTime(1980, 1, 1).AddDays(i % 12000);
                var photoUrl = i % 4 == 0 ? $"https://i.pravatar.cc/150?u=person-{i}" : null;
                var notes = i % 7 == 0 ? $"Observações da pessoa {i}" : null;
                var state = states[random.Next(states.Count)];

                var person = new PersonModel
                {
                    CompanyId = companyId,
                    Name = name,
                    Document = document,
                    Email = email,
                    PhoneNumber = phone,
                    DateOfBirth = dateOfBirth,
                    PhotoUrl = photoUrl,
                    Notes = notes
                };

                context.BasicPeople.Add(person);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedSuppliers(DefaultContext context)
    {
        if (!context.BasicSuppliers.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var people = context.BasicPeople.Take(30).ToList();

            if (!people.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 0; i < 30; i++)
            {
                var supplier = new SupplierModel
                {
                    CompanyId = companyId,
                    PersonId = people[i].Id,
                    Cnpj = ((i % 90000000000000) + 10000000000000).ToString("D14"),
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.BasicSuppliers.Add(supplier);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedCustomers(DefaultContext context)
    {
        if (!context.BasicCustomers.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var people = context.BasicPeople.Skip(100).Take(50).ToList();

            if (people.Count < 50)
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 0; i < 50; i++)
            {
                var customer = new CustomerModel
                {
                    CompanyId = companyId,
                    PersonId = people[i].Id,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.BasicCustomers.Add(customer);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedEmployees(DefaultContext context)
    {
        if (!context.BasicEmployees.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var people = context.BasicPeople.Skip(150).Take(80).ToList();
            var positions = context.BasicPositions.ToList();

            if (!people.Any() || !positions.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 0; i < people.Count; i++)
            {
                var employee = new EmployeeModel
                {
                    CompanyId = companyId,
                    PersonId = people[i].Id,
                    PositionId = positions[random.Next(positions.Count)].Id,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.BasicEmployees.Add(employee);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedOrders(DefaultContext context)
    {
        if (!context.BasicOrders.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var customers = context.BasicCustomers.ToList();
            var employees = context.BasicEmployees.ToList();
            var users = context.AuthUsers.ToList();

            if (!customers.Any() || !employees.Any() || !users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 80; i++)
            {
                var saleDate = now.AddDays(-(random.NextDouble() * 365));
                var orderNumber = $"PED-{saleDate.Year}-{i:D5}";

                var order = new OrderModel
                {
                    CompanyId = companyId,
                    OrderNumber = orderNumber[..Math.Min(orderNumber.Length, 40)],
                    UserId = users[(i - 1) % Math.Max(users.Count, 1)].Id,
                    CustomerId = customers[(i - 1) % Math.Max(customers.Count, 1)].Id,
                    EmployeeId = employees[(i - 1) % Math.Max(employees.Count, 1)].Id,
                    TotalAmount = Math.Round((decimal)((random.NextDouble() * 5000) + 100), 2),
                    DiscountAmount = Math.Round((decimal)(random.NextDouble() * 200), 2),
                    TotalQuantity = random.Next(1, 21),
                    SaleDate = saleDate,
                    Status = (OrderStatus)(i % 3),
                    PaymentMethod = (PaymentMethod)(i % 6),
                    Notes = i % 5 == 0 ? $"Observacao do pedido {i}" : null,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.BasicOrders.Add(order);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedOrderDetails(DefaultContext context)
    {
        if (!context.BasicOrderDetails.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var orders = context.BasicOrders.ToList();
            var products = context.BasicProducts.ToList();

            if (!orders.Any() || !products.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 200; i++)
            {
                var price = Math.Round((decimal)((random.NextDouble() * 500) + 50), 2);
                var discountAmount = Math.Round((decimal)(random.NextDouble() * 50), 2);
                var subtotal = Math.Round((decimal)((random.NextDouble() * 500) + 50), 2);
                var quantity = (random.NextDouble() * 5) + 1;

                var orderDetail = new OrderDetailModel
                {
                    CompanyId = companyId,
                    OrderId = orders[(i - 1) % orders.Count].Id,
                    ProductId = products[(i - 1) % products.Count].Id,
                    Price = price,
                    DiscountAmount = discountAmount,
                    Subtotal = subtotal,
                    Quantity = quantity,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.BasicOrderDetails.Add(orderDetail);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedStockMovements(DefaultContext context)
    {
        if (!context.BasicStockMovements.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var products = context.BasicProducts.ToList();
            var customers = context.BasicCustomers.ToList();
            var suppliers = context.BasicSuppliers.ToList();
            var employees = context.BasicEmployees.ToList();
            var orders = context.BasicOrders.ToList();

            if (!products.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 100; i++)
            {
                var price = Math.Round((decimal)((random.NextDouble() * 500) + 50), 2);
                var quantity = (random.NextDouble() * 100) + 1;
                var date = now.AddDays(-(random.NextDouble() * 365));

                string reason;
                switch (i % 4)
                {
                    case 0: reason = "Compra de fornecedor"; break;
                    case 1: reason = "Venda para cliente"; break;
                    case 2: reason = "Ajuste de inventário"; break;
                    default: reason = "Devolução"; break;
                }

                var stockMovement = new StockMovementModel
                {
                    CompanyId = companyId,
                    ProductId = products[(i - 1) % Math.Max(products.Count, 1)].Id,
                    Quantity = quantity,
                    Date = date,
                    Price = price,
                    Type = i % 2 == 0 ? StockMovementType.In : StockMovementType.Out,
                    Reason = reason,
                    CustomerId = i % 3 == 0 && customers.Any() ? customers[(i - 1) % Math.Max(customers.Count, 1)].Id : null,
                    SupplierId = i % 3 == 0 && suppliers.Any() ? suppliers[(i - 1) % Math.Max(suppliers.Count, 1)].Id : null,
                    EmployeeId = i % 3 == 0 && employees.Any() ? employees[(i - 1) % Math.Max(employees.Count, 1)].Id : null,
                    OrderId = i % 3 == 0 && orders.Any() ? orders[(i - 1) % Math.Max(orders.Count, 1)].Id : null,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.BasicStockMovements.Add(stockMovement);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedPersonAddresses(DefaultContext context)
    {
        if (!context.BasicPersonAddresses.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var people = context.BasicPeople.ToList();
            var addresses = context.AuthAddresses.ToList();

            if (!people.Any() || !addresses.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 0; i < 100; i++)
            {
                var personAddress = new PersonAddressModel
                {
                    CompanyId = companyId,
                    PersonId = people[i % 200].Id,
                    AddressId = addresses[i % 100].Id,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.BasicPersonAddresses.Add(personAddress);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedUser(DefaultContext context)
    {
        if (!context.AuthUsers.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var user = new UserModel
            {
                Email = "ygor@ygorlazaro.com",
                Name = "Ygor Lazaro",
                Password = SecurityService.Hash("ygor")
            };

            var currentRole = context.AuthRoles.FirstOrDefault(x => x.Name == "God");
            var role = new UserRoleModel
            {
                User = user,
                CompanyId = companyId,
                RoleId = currentRole?.Id ?? Guid.Empty
            };

            context.AuthUsers.Add(user);
            context.AuthUserRoles.Add(role);
            context.SaveChanges();
        }
    }

    private static void SeedAuthOrders(DefaultContext context)
    {
        if (!context.AuthOrders.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var users = context.AuthUsers.ToList();

            if (!users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 40; i++)
            {
                var saleDate = now.AddDays(-(random.NextDouble() * 365));
                var orderNumber = $"AUTH-{saleDate.Year}-{i:D5}";

                var authOrder = new AuthOrderModel
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
#pragma warning restore CA5394
        }
    }

    private static void SeedAuthOrderDetails(DefaultContext context)
    {
        if (!context.AuthOrderDetails.Any())
        {
            var orders = context.AuthOrders.ToList();
            var modules = context.AuthModules.ToList();

            if (!orders.Any() || !modules.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 100; i++)
            {
                var price = Math.Round((decimal)((random.NextDouble() * 500) + 50), 2);
                var discountAmount = Math.Round((decimal)(random.NextDouble() * 50), 2);
                var subtotal = Math.Round((decimal)((random.NextDouble() * 500) + 50), 2);

                var authOrderDetail = new AuthOrderDetailModel
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
#pragma warning restore CA5394
        }
    }

    private static void SeedSubscriptions(DefaultContext context)
    {
        if (!context.AuthSubscriptions.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var orders = context.AuthOrders.ToList();

            if (!orders.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
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
#pragma warning restore CA5394
        }
    }

    private static void SeedSubscriptionCredits(DefaultContext context)
    {
        if (!context.AuthSubscriptionCredits.Any())
        {
            var subscriptions = context.AuthSubscriptions.ToList();
            var modules = context.AuthModules.ToList();
            var orderDetails = context.AuthOrderDetails.ToList();

            if (!subscriptions.Any() || !modules.Any() || !orderDetails.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
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
#pragma warning restore CA5394
        }
    }

    private static void SeedConfigurations(DefaultContext context)
    {
        if (!context.AuthConfigurations.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var users = context.AuthUsers.ToList();

            if (!users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
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
#pragma warning restore CA5394
        }
    }

    private static void SeedNotifications(DefaultContext context)
    {
        if (!context.AuthNotifications.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
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
#pragma warning restore CA5394
        }
    }

    private static void SeedForgottenPasswords(DefaultContext context)
    {
        if (!context.AuthForgottenPasswords.Any())
        {
            var users = context.AuthUsers.ToList();

            if (!users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 20; i++)
            {
                var expirationDate = now.AddDays(random.NextDouble() * 7);

                var forgotPassword = new ForgotPasswordModel
                {
                    UserId = users[(i - 1) % users.Count].Id,
                    Code = $"code-{i}",
                    ExpirationDate = expirationDate,
                    IsActive = i % 2 == 0,
                    IpAddress = $"192.168.{(i % 255) + 1}.{(i % 255) + 1}",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
                    Created = now.AddDays(-random.NextDouble() * 7),
                    Updated = now.AddDays(-random.NextDouble() * 7)
                };

                context.AuthForgottenPasswords.Add(forgotPassword);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedUploads(DefaultContext context)
    {
        if (!context.AuthUploads.Any())
        {
            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
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
#pragma warning restore CA5394
        }
    }

    private static void SeedProfiles(DefaultContext context)
    {
        if (!context.SocialNetworkProfiles.Any())
        {
            var users = context.AuthUsers.ToList();
            var uploads = context.AuthUploads.ToList();

            if (!users.Any() || !uploads.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 100; i++)
            {
                var birthDate = new DateTime(1990, 1, 1).AddDays(i % 12000);

                var profile = new ProfileModel
                {
                    UserId = users[(i - 1) % users.Count].Id,
                    UserName = $"usuario{i}",
                    Bio = i % 3 == 0 ? $"Bio do usuário {i} apaixonado por tecnologia e inovação." : null,
                    UploadId = uploads[(i - 1) % uploads.Count].Id,
                    Website = i % 4 == 0 ? $"https://usuario{i}.dev" : null,
                    Location = i % 2 == 0 ? "São Paulo, SP" : "Rio de Janeiro, RJ",
                    Phone = $"({(i % 90) + 10}) 9{(i % 90000000) + 10000000:D8}",
                    BirthDate = birthDate,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.SocialNetworkProfiles.Add(profile);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedFeeds(DefaultContext context)
    {
        if (!context.SocialNetworkFeeds.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var profiles = context.SocialNetworkProfiles.ToList();

            if (!profiles.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 200; i++)
            {
                var profile = profiles[(i - 1) % profiles.Count];

                string text;
                switch (i % 5)
                {
                    case 0: text = $"Post {i} - Aprendendo novas tecnologias!"; break;
                    case 1: text = $"Post {i} - Finalizando sprint com sucesso!"; break;
                    case 2: text = $"Post {i} - Novo projeto na area!"; break;
                    case 3: text = $"Post {i} - Reflexao sobre agile..."; break;
                    default: text = $"Post {i} - Compartilhando conhecimento!"; break;
                }

                var feed = new FeedModel
                {
                    CompanyId = companyId,
                    ProfileId = profile.Id,
                    Text = text,
                    Date = now.AddDays(-(random.NextDouble() * 365)),
                    TotalLikes = (int)(random.NextDouble() * 100),
                    TotalComments = (int)(random.NextDouble() * 30),
                    TotalShares = (int)(random.NextDouble() * 20),
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.SocialNetworkFeeds.Add(feed);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedSocialComments(DefaultContext context)
    {
        if (!context.SocialNetworkComments.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var profiles = context.SocialNetworkProfiles.ToList();
            var feeds = context.SocialNetworkFeeds.ToList();

            if (!profiles.Any() || !feeds.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 300; i++)
            {
                var profile = profiles[(i - 1) % profiles.Count];
                var feed = feeds[(i - 1) % feeds.Count];

                string text;
                switch (i % 3)
                {
                    case 0: text = $"Comentário {i} — Concordo totalmente!"; break;
                    case 1: text = $"Comentário {i} — Interessante ponto de vista."; break;
                    default: text = $"Comentário {i} — Obrigado por compartilhar!"; break;
                }

                var comment = new CommentModel
                {
                    CompanyId = companyId,
                    ProfileId = profile.Id,
                    FeedId = feed.Id,
                    Text = text,
                    CommentDate = now.AddDays(-(random.NextDouble() * 365)),
                    UpdatedDate = i % 4 == 0 ? now.AddDays(-(random.NextDouble() * 30)) : null,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.SocialNetworkComments.Add(comment);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedSocialLikes(DefaultContext context)
    {
        if (!context.SocialNetworkLikes.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var profiles = context.SocialNetworkProfiles.ToList();
            var feeds = context.SocialNetworkFeeds.ToList();

            if (!profiles.Any() || !feeds.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 400; i++)
            {
                var like = new LikeModel
                {
                    CompanyId = companyId,
                    ProfileId = profiles[(i - 1) % profiles.Count].Id,
                    FeedId = feeds[(i - 1) % feeds.Count].Id,
                    LikeDate = now.AddDays(-(random.NextDouble() * 365)),
                    Created = now.AddDays(-random.NextDouble() * 30),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.SocialNetworkLikes.Add(like);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedSocialShares(DefaultContext context)
    {
        if (!context.SocialNetworkShares.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var profiles = context.SocialNetworkProfiles.ToList();
            var feeds = context.SocialNetworkFeeds.ToList();

            if (!profiles.Any() || !feeds.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 50; i++)
            {
                var share = new ShareModel
                {
                    CompanyId = companyId,
                    ProfileId = profiles[(i - 1) % profiles.Count].Id,
                    OriginalFeedId = feeds[(i - 1) % feeds.Count].Id,
                    Text = i % 2 == 0 ? "Compartilhando com meu comentário..." : null,
                    ShareDate = now.AddDays(-(random.NextDouble() * 365)),
                    Created = now.AddDays(-random.NextDouble() * 30),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.SocialNetworkShares.Add(share);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedSocialBlocks(DefaultContext context)
    {
        if (!context.SocialNetworkBlocks.Any())
        {
            var profiles = context.SocialNetworkProfiles.ToList();

            if (!profiles.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 10; i++)
            {
                var profileIndex = (i - 1) % profiles.Count;
                var blockedProfileIndex = (((i - 1) % 100) + 1) % 100;

                string reason;
                switch (i % 3)
                {
                    case 0: reason = "Conteúdo impróprio"; break;
                    case 1: reason = "Spam"; break;
                    default: reason = "Assédio"; break;
                }

                var block = new BlockModel
                {
                    ProfileId = profiles[profileIndex].Id,
                    BlockedProfileId = profiles[blockedProfileIndex].Id,
                    Reason = reason,
                    BlockDate = now.AddDays(-(random.NextDouble() * 180)),
                    IsActive = i % 2 == 0,
                    Created = now.AddDays(-random.NextDouble() * 180),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.SocialNetworkBlocks.Add(block);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedSocialReports(DefaultContext context)
    {
        if (!context.SocialNetworkReports.Any())
        {
            var users = context.AuthUsers.ToList();
            var profiles = context.SocialNetworkProfiles.ToList();

            if (!users.Any() || !profiles.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 20; i++)
            {
                var targetType = i % 2 == 0 ? "profile" : "feed";
                var targetId = profiles[(i - 1) % profiles.Count].Id;

                string reason;
                switch (i % 3)
                {
                    case 0: reason = "Spam"; break;
                    case 1: reason = "Conteúdo ofensivo"; break;
                    default: reason = "Assédio"; break;
                }

                var report = new ReportModel
                {
                    ReporterId = users[(i - 1) % users.Count].Id,
                    TargetId = targetId,
                    TargetType = targetType,
                    Reason = reason,
                    Description = $"Descrição detalhada da denúncia {i}",
                    Status = (EnumReportStatus)(i % 3),
                    ReportDate = now.AddDays(-(random.NextDouble() * 180)),
                    Created = now.AddDays(-random.NextDouble() * 180),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.SocialNetworkReports.Add(report);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedSocialAttachments(DefaultContext context)
    {
        if (!context.SocialNetworkAttachments.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var comments = context.SocialNetworkComments.ToList();

            if (!comments.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 50; i++)
            {
                var fileSize = (long)((random.NextDouble() * 2000000) + 100000);

                var attachment = new SocialAttachmentModel
                {
                    CompanyId = companyId,
                    CommentId = comments[(i - 1) % comments.Count].Id,
                    Url = $"https://fenicia.s3.amazonaws.com/social/attachment-{i}.jpg",
                    FileType = "image/jpeg",
                    FileSize = fileSize,
                    UploadDate = now.AddDays(-(random.NextDouble() * 365)),
                    Created = now.AddDays(-random.NextDouble() * 30),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.SocialNetworkAttachments.Add(attachment);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedProjects(DefaultContext context)
    {
        if (!context.Projects.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var users = context.AuthUsers.ToList();

            if (!users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 20; i++)
            {
                var startDate = now.AddDays(-(random.NextDouble() * 365));
                var endDate = now.AddDays(random.NextDouble() * 180);

                string title;
                switch (i % 5)
                {
                    case 0: title = $"Projeto {i} — Plataforma E-commerce"; break;
                    case 1: title = $"Projeto {i} — App Mobile"; break;
                    case 2: title = $"Projeto {i} — Sistema de CRM"; break;
                    case 3: title = $"Projeto {i} — Portal do Cliente"; break;
                    default: title = $"Projeto {i} — Integração API"; break;
                }

                var project = new ProjectModel
                {
                    CompanyId = companyId,
                    Title = title,
                    Description = $"Descrição do projeto {i} com escopo, objetivos e entregas definidas.",
                    Status = (EnumProjectStatus)(i % 4),
                    StartDate = startDate,
                    EndDate = endDate,
                    Owner = users[(i - 1) % users.Count].Id,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.Projects.Add(project);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedProjectStatuses(DefaultContext context)
    {
        if (!context.ProjectStatuses.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var projects = context.Projects.ToList();

            if (!projects.Any())
            {
                return;
            }

            var statuses = new[]
            {
                new { Name = "Backlog", Color = "#6c757d", Order = 1, IsFinal = false },
                new { Name = "A Fazer", Color = "#0dcaf0", Order = 2, IsFinal = false },
                new { Name = "Em Progresso", Color = "#ffc107", Order = 3, IsFinal = false },
                new { Name = "Em Revisão", Color = "#fd7e14", Order = 4, IsFinal = false },
                new { Name = "Concluído", Color = "#198754", Order = 5, IsFinal = true },
                new { Name = "Bloqueado", Color = "#dc3545", Order = 6, IsFinal = false }
            };

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 60; i++)
            {
                var statusIndex = (i - 1) % 6;
                var project = projects[(i - 1) % projects.Count];

                var projectStatus = new ProjectStatusModel
                {
                    CompanyId = companyId,
                    ProjectId = project.Id,
                    Name = statuses[statusIndex].Name,
                    Color = statuses[statusIndex].Color,
                    Order = statuses[statusIndex].Order,
                    IsFinal = statuses[statusIndex].IsFinal,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.ProjectStatuses.Add(projectStatus);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedSprints(DefaultContext context)
    {
        if (!context.Sprints.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var projects = context.Projects.ToList();
            var users = context.AuthUsers.ToList();

            if (!projects.Any() || !users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 40; i++)
            {
                var startDate = now.AddDays(-(random.NextDouble() * 180));
                var endDate = now.AddDays(random.NextDouble() * 180);

                var sprint = new SprintModel
                {
                    CompanyId = companyId,
                    ProjectId = projects[(i - 1) % projects.Count].Id,
                    Name = $"Sprint {i}",
                    Description = $"Sprint {i} do projeto {(i % 20) + 1}",
                    StartDate = startDate,
                    EndDate = endDate,
                    CreatedBy = users[(i - 1) % users.Count].Id,
                    Created = now.AddDays(-random.NextDouble() * 180),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.Sprints.Add(sprint);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedTasks(DefaultContext context)
    {
        if (!context.ProjectTasks.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var projects = context.Projects.ToList();
            var projectStatuses = context.ProjectStatuses.ToList();
            var users = context.AuthUsers.ToList();
            var sprints = context.Sprints.ToList();

            if (!projects.Any() || !projectStatuses.Any() || !users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 100; i++)
            {
                var dueDate = now.AddDays(random.NextDouble() * 90);
                var sprintId = i % 2 == 0 && sprints.Any() ? (Guid?)sprints[(i - 1) % sprints.Count].Id : null;

                string title;
                switch (i % 5)
                {
                    case 0: title = $"Task {i} — Implementar login"; break;
                    case 1: title = $"Task {i} — Corrigir bug no checkout"; break;
                    case 2: title = $"Task {i} — Criar dashboard"; break;
                    case 3: title = $"Task {i} — Otimizar query"; break;
                    default: title = $"Task {i} — Refatorar módulo"; break;
                }

                var task = new ProjectTaskModel
                {
                    CompanyId = companyId,
                    ProjectId = projects[(i - 1) % projects.Count].Id,
                    StatusId = projectStatuses[(i - 1) % projectStatuses.Count].Id,
                    Title = title,
                    Description = $"Descrição da task {i} com detalhes de implementação.",
                    Priority = (EnumTaskPriority)(i % 4),
                    Type = (EnumTaskType)(i % 5),
                    Order = i,
                    EstimatePoints = (i % 13) + 1,
                    DueDate = dueDate,
                    CreatedBy = users[(i - 1) % users.Count].Id,
                    SprintId = sprintId,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.ProjectTasks.Add(task);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedProjectSubtasks(DefaultContext context)
    {
        if (!context.ProjectSubtasks.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var tasks = context.ProjectTasks.ToList();

            if (!tasks.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 200; i++)
            {
                var dueDate = now.AddDays(random.NextDouble() * 60);
                var isCompleted = i % 3 == 0;
                var completedAt = isCompleted ? (DateTime?)now.AddDays(-(random.NextDouble() * 30)) : null;

                var subtask = new ProjectSubtaskModel
                {
                    CompanyId = companyId,
                    TaskId = tasks[(i - 1) % tasks.Count].Id,
                    Title = $"Subtask {i} de {(i % 100) + 1}",
                    Order = (i % 5) + 1,
                    IsCompleted = isCompleted,
                    DueDate = dueDate,
                    CompletedAt = completedAt,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.ProjectSubtasks.Add(subtask);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedTaskAssignees(DefaultContext context)
    {
        if (!context.ProjectTaskAssignees.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var tasks = context.ProjectTasks.ToList();
            var users = context.AuthUsers.ToList();

            if (!tasks.Any() || !users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 200; i++)
            {
                var assignedAt = now.AddDays(-(random.NextDouble() * 365));

                var taskAssignee = new TaskAssigneeModel
                {
                    CompanyId = companyId,
                    TaskId = tasks[(i - 1) % tasks.Count].Id,
                    UserId = users[(i - 1) % users.Count].Id,
                    Role = (EnumAssigneeRole)(i % 3),
                    AssignedAt = assignedAt,
                    Created = now.AddDays(-random.NextDouble() * 30),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.ProjectTaskAssignees.Add(taskAssignee);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedProjectComments(DefaultContext context)
    {
        if (!context.ProjectComments.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var tasks = context.ProjectTasks.ToList();
            var users = context.AuthUsers.ToList();

            if (!tasks.Any() || !users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 100; i++)
            {
                var projectComment = new ProjectCommentModel
                {
                    CompanyId = companyId,
                    TaskId = tasks[(i - 1) % tasks.Count].Id,
                    UserId = users[(i - 1) % users.Count].Id,
                    AuthorId = users[(i - 1) % users.Count].Id,
                    Content = $"Comentário no task {(i % 100) + 1} pelo usuário {(i % 100) + 1}.",
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.ProjectComments.Add(projectComment);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedProjectAttachments(DefaultContext context)
    {
        if (!context.ProjectAttachments.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var tasks = context.ProjectTasks.ToList();
            var users = context.AuthUsers.ToList();

            if (!tasks.Any() || !users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 50; i++)
            {
                var fileSize = (long)((random.NextDouble() * 5000000) + 500000);

                var attachment = new ProjectAttachmentModel
                {
                    CompanyId = companyId,
                    TaskId = tasks[(i - 1) % tasks.Count].Id,
                    FileName = $"anexo-{i}.pdf",
                    FileUrl = $"https://fenicia.s3.amazonaws.com/anexos/anexo-{i}.pdf",
                    FileSize = fileSize,
                    Size = fileSize,
                    ContentType = "application/pdf",
                    UploadedBy = users[(i - 1) % users.Count].Id,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.ProjectAttachments.Add(attachment);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedTeams(DefaultContext context)
    {
        if (!context.Teams.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var projects = context.Projects.ToList();
            var users = context.AuthUsers.ToList();

            if (!projects.Any() || !users.Any())
            {
                return;
            }

            var teamNames = new[]
            {
                "Frontend", "Backend", "Mobile", "QA", "DevOps"
            };

            var teamColors = new[]
            {
                "#0d6efd", "#198754", "#ffc107", "#dc3545", "#6f42c1"
            };

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 20; i++)
            {
                var teamIndex = i % 5;

                var team = new TeamModel
                {
                    CompanyId = companyId,
                    ProjectId = projects[(i - 1) % projects.Count].Id,
                    Name = $"Time {i} — {teamNames[teamIndex]}",
                    Description = $"Time responsável pela squad {i}",
                    Color = teamColors[teamIndex],
                    CreatedBy = users[(i - 1) % users.Count].Id,
                    Created = now.AddDays(-random.NextDouble() * 365),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.Teams.Add(team);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedTeamUsers(DefaultContext context)
    {
        if (!context.ProjectTeamUsers.Any())
        {
            var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
            var teams = context.Teams.ToList();
            var users = context.AuthUsers.ToList();

            if (!teams.Any() || !users.Any())
            {
                return;
            }

            var random = new Random();
            var now = DateTime.UtcNow;

#pragma warning disable CA5394
            for (var i = 1; i <= 60; i++)
            {
                var joinedAt = now.AddDays(-(random.NextDouble() * 365));

                var teamUser = new TeamUserModel
                {
                    CompanyId = companyId,
                    TeamId = teams[(i - 1) % teams.Count].Id,
                    UserId = users[(i - 1) % users.Count].Id,
                    Role = (EnumTeamRole)(i % 2),
                    JoinedAt = joinedAt,
                    Created = now.AddDays(-random.NextDouble() * 30),
                    Updated = now.AddDays(-random.NextDouble() * 30)
                };

                context.ProjectTeamUsers.Add(teamUser);
            }
#pragma warning restore CA5394
        }
    }

    private static void SeedCompanies(DefaultContext context)
    {
        if (!context.AuthCompanies.Any())
        {
            context.AuthCompanies.Add(new CompanyModel
            {
                Id = Guid.Parse("d6f7e2c6-2986-47a5-8884-c3c249546ba7"),
                Name = "Gato Ninja",
                Cnpj = "23351185000184"
            });

            context.SaveChanges();
        }
    }

    private static void SeedRoles(DefaultContext context)
    {
        if (!context.AuthRoles.Any())
        {
            context.AuthRoles.AddRange(
                new RoleModel { Name = "God" },
                new RoleModel { Name = "Admin" },
                new RoleModel { Name = "User" });
        }
    }

    private static void SeedStates(DefaultContext context)
    {
        if (!context.AuthStates.Any())
        {
            var states = new List<StateModel>
            {
                new() { Name = "Acre", Uf = "AC" },
                new() { Name = "Alagoas", Uf = "AL" },
                new() { Name = "Amapá", Uf = "AP" },
                new() { Name = "Amazonas", Uf = "AM" },
                new() { Name = "Bahia", Uf = "BA" },
                new() { Name = "Ceará", Uf = "CE" },
                new() { Name = "Distrito Federal", Uf = "DF" },
                new() { Name = "Espírito Santo", Uf = "ES" },
                new() { Name = "Goiás", Uf = "GO" },
                new() { Name = "Maranhão", Uf = "MA" },
                new() { Name = "Mato Grosso", Uf = "MT" },
                new() { Name = "Mato Grosso do Sul", Uf = "MS" },
                new() { Name = "Minas Gerais", Uf = "MG" },
                new() { Name = "Pará", Uf = "PA" },
                new() { Name = "Paraíba", Uf = "PB" },
                new() { Name = "Paraná", Uf = "PR" },
                new() { Name = "Pernambuco", Uf = "PE" },
                new() { Name = "Piauí", Uf = "PI" },
                new() { Name = "Rio de Janeiro", Uf = "RJ" },
                new() { Name = "Rio Grande do Norte", Uf = "RN" },
                new() { Name = "Rio Grande do Sul", Uf = "RS" },
                new() { Name = "Rondônia", Uf = "RO" },
                new() { Name = "Roraima", Uf = "RR" },
                new() { Name = "Santa Catarina", Uf = "SC" },
                new() { Name = "São Paulo", Uf = "SP" },
                new() { Name = "Sergipe", Uf = "SE" },
                new() { Name = "Tocantins", Uf = "TO" }
            };

            foreach (var state in states)
            {
                context.AuthStates.Add(state);
            }
        }
    }

    private static void SeedModules(DefaultContext context)
    {
        if (!context.AuthModules.Any())
        {
            var modules = new List<ModuleModel>();
            var sortOrder = 1;

            foreach (var moduleType in Enum.GetValues<ModuleType>())
            {
                modules.Add(new ModuleModel
                {
                    Name = moduleType.ToString(),
                    Type = moduleType,
                    Price = 30m,
                    IsActive = true,
                    SortOrder = sortOrder++,
                    Description = $"Module {moduleType}"
                });
            }

            foreach (var module in modules)
            {
                context.AuthModules.Add(module);
            }
        }
    }
}
