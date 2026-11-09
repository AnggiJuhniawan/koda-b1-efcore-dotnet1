// using KodaStore.Api.Data;
// using KodaStore.Api.DTOs;
// using KodaStore.Api.Models;
// using KodaStore.Api.Services;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.FileSystemGlobbing;

// namespace KodaStore.Api.Controllers;

// [ApiController]
// [Route("api/v1/products")]

// public sealed class ProductController : ControllerBase
// {
//     private readonly IProductService _service;

//     public ProductController(IProductService service)
//     {
//         _service = service;
//     }

//     [HttpGet]
//     [ProducesResponseType(
//         typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
//     public ActionResult<IReadOnlyList<Product>> GetAll()
//     {
//         return Ok(_service.GetAll());
//     }

//     [HttpGet("{productCode}")]
//     [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//     public ActionResult<ProductResponse> GetByCode(
//         [FromRoute] string productCode)
//     {
//         var product = _service.GetByCode(productCode);

//         return product is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found") : Ok(product);
//     }
//     // HTTPPOST
//     [HttpPost]
//     [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//     public ActionResult<ProductResponse> Create([FromBody] ProductRequest request)
//     {
//         try
//         {
//             // Map DTO ke Model Internal
//             var product = _service.Create(request);

//             if (product is null)
//             {
//                 return Problem(statusCode: StatusCodes.Status409Conflict, title: "Product code already exists");
//             }

//             // Kembalikan response 201
//             return CreatedAtAction(
//                 nameof(GetByCode),
//                 new { productCode = product.ProductCode }, product);
//         }
//         catch (ArgumentException e)
//         {
//             return Problem(
//                 statusCode: StatusCodes.Status400BadRequest,
//                 title: "Invalid product data!",
//                 detail: e.Message);
//         }
//     }

//     [HttpDelete("{productCode}")]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//     public IActionResult Delete([FromRoute] string productCode)
//     {
//         return _service.Delete(productCode) ? NoContent() : Problem(
//             statusCode: StatusCodes.Status404NotFound,
//             title: "Product not found",
//             detail: $"No product was found for code {productCode}.");
//     }

//     [HttpPut("{productCode}")]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//     [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//     public ActionResult<ProductResponse> Update(
//         [FromRoute] string productCode, [FromBody] UpdateProductRequest request)
//     {
//         try
//         {
//             var exitingProduct = _service.Update(productCode, request);

//             return exitingProduct is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found", detail: $"No product was found for code {productCode}.") : Ok(exitingProduct);
//         }
//         catch (ArgumentException e)
//         {
//             return Problem(
//                 statusCode: StatusCodes.Status400BadRequest,
//                 title: "Invalid product data!",
//                 detail: e.Message);
//         }
//     }
// }