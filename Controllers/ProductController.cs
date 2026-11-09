using KodaStore.Api.DTOs;
using KodaStore.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KodaStore.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    // GET /api/v1/products
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ProductResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var products = await _service.GetAllAsync(
            cancellationToken);

        return Ok(products);
    }

    // GET /api/v1/products/PR001
    [HttpGet("{productCode}")]
    [ProducesResponseType(
        typeof(ProductResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetByCode(
        [FromRoute] string productCode,
        CancellationToken cancellationToken)
    {
        var product = await _service.GetByCodeAsync(
            productCode,
            cancellationToken);

        if (product is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Product not found");
        }

        return Ok(product);
    }

    // POST /api/v1/products
    [HttpPost]
    [ProducesResponseType(
        typeof(ProductResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponse>> Create(
        [FromBody] ProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateAsync(
                request,
                cancellationToken);

            if (created is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Product code already exists");
            }

            return CreatedAtAction(
                nameof(GetByCode),
                new { productCode = created.ProductCode },
                created);
        }
        catch (ArgumentException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid product data",
                detail: ex.Message);
        }
    }

    // PUT /api/v1/products/PR001
    [HttpPut("{productCode}")]
    [ProducesResponseType(
        typeof(ProductResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> Update(
        [FromRoute] string productCode,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _service.UpdateAsync(
                productCode,
                request,
                cancellationToken);

            if (updated is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Product not found");
            }

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid product data",
                detail: ex.Message);
        }
    }

    // DELETE /api/v1/products/PR001
    [HttpDelete("{productCode}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] string productCode,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(
            productCode,
            cancellationToken);

        if (!deleted)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Product not found");
        }

        return NoContent();
    }
}