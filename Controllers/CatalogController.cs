
using KodaStore.Api.DTOs;
using KodaStore.Api.Services;
using KodaStore.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KodaStore.Api.Controllers;

[Route("catalog")]
public sealed class CatalogController : Controller
{
    private readonly IProductService _service;

    // Constructor Injection
    public CatalogController(IProductService service)
    {
        _service = service;
    }

    // ========================================
    // 1. GET ALL PRODUCTS
    // GET /catalog
    // ========================================
    [HttpGet("")]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var products = await _service.GetAllAsync(
            cancellationToken);

        return View(products);
    }

    // ========================================
    // 2. GET PRODUCT DETAILS
    // GET /catalog/details/PR001
    // ========================================
    [HttpGet("details/{productCode}")]
    public async Task<IActionResult> Details(
        string productCode,
        CancellationToken cancellationToken)
    {
        var product = await _service.GetByCodeAsync(
            productCode,
            cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    // ========================================
    // 3. SHOW CREATE FORM
    // GET /catalog/create
    // ========================================
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new CreateProductViewModel());
    }

    // ========================================
    // 4. PROCESS CREATE FORM
    // POST /catalog/create
    // ========================================
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateProductViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Mapping ViewModel -> DTO
            var request = new ProductRequest(
    model.ProductCode,
    model.Name,
    model.Price ?? 0, // Jika null, anggap saja 0 (meski tidak mungkin null karena ModelState)
    model.Stock ?? 0, // Jika null, anggap saja 0
    model.Category
);

            // Service -> Repository -> SQL Server
            var created = await _service.CreateAsync(
                request,
                cancellationToken);

            // ProductCode already exists
            if (created is null)
            {
                ModelState.AddModelError(
                    nameof(model.ProductCode),
                    "Kode produk sudah digunakan.");

                return View(model);
            }

            // Success message
            TempData["Success"] =
                "Produk berhasil ditambahkan.";

            // Redirect after successful POST
            return RedirectToAction(
                nameof(Details),
                new { productCode = created.ProductCode });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(model);
        }
    }

    // ========================================
    // 5. SHOW EDIT FORM
    // GET /catalog/edit/PR001
    // ========================================
    [HttpGet("edit/{productCode}")]
    public async Task<IActionResult> Edit(
        string productCode,
        CancellationToken cancellationToken)
    {
        var product = await _service.GetByCodeAsync(
            productCode,
            cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        ViewData["ProductCode"] = product.ProductCode;

        // Mapping Response DTO -> ViewModel
        var model = new EditProductViewModel
        {
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category
        };

        return View(model);
    }

    // ========================================
    // 6. PROCESS EDIT FORM
    // POST /catalog/edit/PR001
    // ========================================
    [HttpPost("edit/{productCode}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        string productCode,
        EditProductViewModel model,
        CancellationToken cancellationToken)
    {
        ViewData["ProductCode"] = productCode;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Mapping ViewModel -> DTO
            var request = new UpdateProductRequest
            (
                model.Name,
                model.Price ?? 0, // Jika null, anggap saja 0 (meski tidak mungkin null karena ModelState)
                model.Stock ?? 0, // Jika null, anggap saja 0
                model.Category
            );

            // Service -> Repository -> SQL UPDATE
            var updated = await _service.UpdateAsync(
                productCode,
                request,
                cancellationToken);

            if (updated is null)
            {
                return NotFound();
            }

            TempData["Success"] =
                "Produk berhasil diperbarui.";

            return RedirectToAction(
                nameof(Details),
                new { productCode = updated.ProductCode });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(model);
        }
    }

    // ========================================
    // 7. SHOW DELETE CONFIRMATION
    // GET /catalog/delete/PR001
    // ========================================
    [HttpGet("delete/{productCode}")]
    public async Task<IActionResult> Delete(
        string productCode,
        CancellationToken cancellationToken)
    {
        var product = await _service.GetByCodeAsync(
            productCode,
            cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    // ========================================
    // 8. PROCESS DELETE
    // POST /catalog/delete/PR001
    // ========================================
    [HttpPost("delete/{productCode}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        string productCode,
        CancellationToken cancellationToken)
    {
        // Service -> Repository -> SQL DELETE
        var deleted = await _service.DeleteAsync(
            productCode,
            cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["Success"] =
            "Produk berhasil dihapus.";

        return RedirectToAction(nameof(Index));
    }
}
