// using KodaStore.Api.Data;
// using KodaStore.Api.Models;

// namespace KodaStore.Api.Repositories;

// public sealed class InMemoryProductRepository : IProductRepository
// {
//     private readonly InMemoryProductCatalog _catalog;

//     public InMemoryProductRepository(InMemoryProductCatalog catalog)
//     {
//         _catalog = catalog;
//     }
//     public IReadOnlyList<Product> GetAll() => _catalog.GetAll();

//     public Product? GetByCode(string productCode) => _catalog.GetByCode(productCode);

//     public bool TryAdd(Product product) => _catalog.TryCreateProduct(product);

//     public bool TryDelete(string productCode) => _catalog.TryRemove(productCode);

//     public bool TryUpdate(Product product) => _catalog.TryUpdateProduct(product);
// }