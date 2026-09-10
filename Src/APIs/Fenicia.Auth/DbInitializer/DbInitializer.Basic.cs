using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Auth.DbInitializer;

internal static partial class DbInitializer
{
    private static void SeedProductCategories(DefaultContext context)
    {
        if (context.BasicProductCategories.Any())
        {
            return;
        }

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

        foreach (var category in categoryNames.Select(categoryName => new ProductCategoryModel
                 {
                     Name = categoryName,
                     CompanyId = companyId
                 }))
        {
            context.BasicProductCategories.Add(category);
        }
    }

    private static void SeedProducts(DefaultContext context)
    {
        if (context.BasicProducts.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var categories = context.BasicProductCategories.ToList();
        var suppliers = context.BasicSuppliers.ToList();

        if (!categories.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 100; i++)
        {
            var category = categories[(i - 1) % categories.Count];

            var name = (i % 10) switch
            {
                0 => $"Produto {i} — Smartphone",
                1 => $"Produto {i} — Notebook",
                2 => $"Produto {i} — Monitor",
                3 => $"Produto {i} — Teclado",
                4 => $"Produto {i} — Mouse",
                5 => $"Produto {i} — Cadeira",
                6 => $"Produto {i} — Mesa",
                7 => $"Produto {i} — Impressora",
                8 => $"Produto {i} — Headset",
                _ => $"Produto {i} — Webcam"
            };

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
    }

    private static void SeedPosition(DefaultContext context)
    {
        if (context.BasicPositions.Any())
        {
            return;
        }

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

        foreach (var position in positionNames.Select(positionName => new PositionModel
                 {
                     Name = positionName,
                     CompanyId = companyId
                 }))
        {
            context.BasicPositions.Add(position);
        }

        context.SaveChanges();
    }

    private static void SeedSuppliers(DefaultContext context)
    {
        if (context.BasicSuppliers.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var people = context.BasicPeople.Take(30).ToList();

        if (!people.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

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
    }

    private static void SeedCustomers(DefaultContext context)
    {
        if (context.BasicCustomers.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var people = context.BasicPeople.Skip(100).Take(50).ToList();

        if (people.Count < 50)
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

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
    }

    private static void SeedEmployees(DefaultContext context)
    {
        if (context.BasicEmployees.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var people = context.BasicPeople.Skip(150).Take(80).ToList();
        var positions = context.BasicPositions.ToList();

        if (!people.Any() || !positions.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        foreach (var employee in people.Select(person => new EmployeeModel
                 {
                     CompanyId = companyId,
                     PersonId = person.Id,
                     PositionId = positions[random.Next(positions.Count)].Id,
                     Created = now.AddDays(-random.NextDouble() * 365),
                     Updated = now.AddDays(-random.NextDouble() * 30)
                 }))
        {
            context.BasicEmployees.Add(employee);
        }
    }

    private static void SeedOrders(DefaultContext context)
    {
        if (context.BasicOrders.Any())
        {
            return;
        }

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
    }

    private static void SeedOrderDetails(DefaultContext context)
    {
        if (context.BasicOrderDetails.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var orders = context.BasicOrders.ToList();
        var products = context.BasicProducts.ToList();

        if (!orders.Any() || !products.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

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
    }

    private static void SeedStockMovements(DefaultContext context)
    {
        if (context.BasicStockMovements.Any())
        {
            return;
        }

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

        for (var i = 1; i <= 100; i++)
        {
            var price = Math.Round((decimal)((random.NextDouble() * 500) + 50), 2);
            var quantity = (random.NextDouble() * 100) + 1;
            var date = now.AddDays(-(random.NextDouble() * 365));

            var reason = (i % 4) switch
            {
                0 => "Compra de fornecedor",
                1 => "Venda para cliente",
                2 => "Ajuste de inventário",
                _ => "Devolução"
            };

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
    }
}
