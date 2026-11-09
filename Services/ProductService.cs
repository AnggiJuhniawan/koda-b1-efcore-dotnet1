using KodaStore.Api.DTOs;
using KodaStore.Api.Models;
using KodaStore.Api.Repositories;

namespace KodaStore.Api.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    // 1. READ ALL
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync(
            cancellationToken);

        return products
            .Select(ProductResponse.FromModel)
            .ToArray();
    }

    // 2. READ DETAIL
    public async Task<ProductResponse?> GetByCodeAsync(
        string productCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productCode)
            || productCode.Trim().Length > 30)
        {
            return null;
        }

        var product = await _repository.GetByCodeAsync(
            productCode,
            cancellationToken);

        return product is null
            ? null
            : ProductResponse.FromModel(product);
    }

    // 3. CREATE
    public async Task<ProductResponse?> CreateAsync(
        ProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = new Product(
            request.ProductCode,
            request.Name,
            request.Price,
            request.Stock,
            request.Category);

        var created = await _repository.TryAddAsync(
            product,
            cancellationToken);

        return created
            ? ProductResponse.FromModel(product)
            : null;
    }

    // 4. UPDATE
    public async Task<ProductResponse?> UpdateAsync(
        string productCode,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = new Product(
            productCode,
            request.Name,
            request.Price,
            request.Stock,
            request.Category);

        var updated = await _repository.TryUpdateAsync(
            product,
            cancellationToken);

        return updated
            ? ProductResponse.FromModel(product)
            : null;
    }

    // 5. DELETE
    public Task<bool> DeleteAsync(
        string productCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productCode)
            || productCode.Trim().Length > 30)
        {
            return Task.FromResult(false);
        }

        return _repository.TryDeleteAsync(
            productCode,
            cancellationToken);
    }
}