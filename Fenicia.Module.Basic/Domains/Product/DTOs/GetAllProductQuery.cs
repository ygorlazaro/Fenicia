namespace Fenicia.Module.Basic.Domains.Product.DTOs;

public record GetAllProductQuery(

    int Page = 1,

    int PerPage = 10);
