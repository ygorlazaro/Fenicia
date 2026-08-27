using MediatR;

namespace Fenicia.Auth.Domains.Company.Queries;

public record CheckCompanyExistsQuery(string Cnpj, bool OnlyActive) : IRequest<bool>;
