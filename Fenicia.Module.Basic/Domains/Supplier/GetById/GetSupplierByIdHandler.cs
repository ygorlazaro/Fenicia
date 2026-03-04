using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.GetById;

public class GetSupplierByIdHandler(DefaultContext context)
{
    public async Task<GetSupplierByIdResponse?> Handle(GetSupplierByIdQuery query, CancellationToken ct)
    {
        var supplier = await context.BasicSuppliers
            .Include(s => s.PersonModel)
            .FirstOrDefaultAsync(s => s.Id == query.Id, ct);

        if (supplier is null)
            return null;

        return new GetSupplierByIdResponse(
            supplier.Id,
            supplier.PersonId,
            supplier.PersonModel.Name,
            supplier.PersonModel.Email,
            supplier.PersonModel.PhoneNumber,
            supplier.PersonModel.Document,
            supplier.PersonModel.Street,
            supplier.PersonModel.Number,
            supplier.PersonModel.Complement,
            supplier.PersonModel.Neighborhood,
            supplier.PersonModel.ZipCode,
            supplier.PersonModel.StateId,
            supplier.PersonModel.City);
    }
}
