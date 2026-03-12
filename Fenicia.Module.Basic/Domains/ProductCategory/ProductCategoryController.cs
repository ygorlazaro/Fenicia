using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Product.Handlers;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Handlers;
using Fenicia.Module.Basic.Domains.ProductCategory.Queries;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProductCategoryController(
    GetAllProductCategoryHandler getAllProductCategoryHandler,
    GetProductCategoryByIdHandler getProductCategoryByIdHandler,
    AddProductCategoryHandler addProductCategoryHandler,
    UpdateProductCategoryHandler updateProductCategoryHandler,
    DeleteProductCategoryHandler deleteProductCategoryHandler,
    GetProductsByCategoryIdHandler getProductsByCategoryIdHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllProductCategoryResponse>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllProductCategoryResponse>>>> GetAsync(
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var productCategory = await getAllProductCategoryHandler.Handle(new GetAllProductCategoryQuery(page,
                perPage),
            ct);

        return Ok(productCategory);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductCategoryByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProductCategoryByIdResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var productCategory = await getProductCategoryByIdHandler.Handle(new GetProductCategoryByIdQuery(id),
            ct);

        return productCategory is null ? NotFound() : Ok(productCategory);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProductCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProductCategoryResponse>> PostAsync(
        [FromBody] AddProductCategoryCommand command,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var productCategory = await addProductCategoryHandler.Handle(command,
            ct);

        return new CreatedResult(string.Empty,
            productCategory);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProductCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProductCategoryResponse>> PatchAsync(
        [FromBody] UpdateProductCategoryCommand command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var productCategory = await updateProductCategoryHandler.Handle(command with
            {
                Id = id
            },
            ct);

        return productCategory is null ? NotFound() : Ok(productCategory);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        await deleteProductCategoryHandler.Handle(new DeleteProductCategoryCommand(id),
            ct);

        return NoContent();
    }

    [HttpGet("{id:guid}/product")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetProductsByCategoryIdResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetProductsByCategoryIdResponse>>> GetProductsByCategoryAsync(
        [FromRoute] Guid categoryId,
        [FromQuery] PaginationQuery query,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var products = await getProductsByCategoryIdHandler.Handle(new GetProductsByCategoryIdQuery(categoryId,
                query.Page,
                query.PerPage),
            ct);

        return Ok(products);
    }
}
