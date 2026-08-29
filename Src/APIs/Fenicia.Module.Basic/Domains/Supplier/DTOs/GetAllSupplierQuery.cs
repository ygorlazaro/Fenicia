namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record GetAllSupplierQuery(

    int Page = 1,

    int PerPage = 10);
