using System.Data;
using KodaStore.Api.Models;
using Microsoft.Data.SqlClient;

namespace KodaStore.Api.Repositories;

public sealed class SqlProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public SqlProductRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("KodaStore")
            ?? throw new InvalidOperationException(
                "Connection string KodaStore is missing.");
    }

    // 1. READ ALL
    public async Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT ProductCode, Name, Price, Stock, Category
            FROM dbo.Products
            ORDER BY ProductCode;
            """;

        var products = new List<Product>();

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command =
            new SqlCommand(sql, connection);

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            products.Add(ReadProduct(reader));
        }

        return products;
    }

    // 2. READ DETAIL
    public async Task<Product?> GetByCodeAsync(
        string productCode,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT ProductCode, Name, Price, Stock, Category
            FROM dbo.Products
            WHERE ProductCode = @productCode;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command =
            new SqlCommand(sql, connection);

        AddProductCode(command, productCode);

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return ReadProduct(reader);
    }

    // 3. CREATE
    public async Task<bool> TryAddAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.Products
                (ProductCode, Name, Price, Stock, Category)
            VALUES
                (@productCode, @name, @price, @stock, @category);
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command =
            new SqlCommand(sql, connection);

        AddProductParameters(command, product);

        try
        {
            var affectedRows =
                await command.ExecuteNonQueryAsync(cancellationToken);

            return affectedRows == 1;
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return false;
        }
    }

    // 4. UPDATE
    public async Task<bool> TryUpdateAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Products
            SET
                Name = @name,
                Price = @price,
                Stock = @stock,
                Category = @category
            WHERE ProductCode = @productCode;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command =
            new SqlCommand(sql, connection);

        AddProductParameters(command, product);

        var affectedRows =
            await command.ExecuteNonQueryAsync(cancellationToken);

        return affectedRows == 1;
    }

    // 5. DELETE
    public async Task<bool> TryDeleteAsync(
        string productCode,
        CancellationToken cancellationToken)
    {
        const string sql = """
            DELETE FROM dbo.Products
            WHERE ProductCode = @productCode;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command =
            new SqlCommand(sql, connection);

        AddProductCode(command, productCode);

        var affectedRows =
            await command.ExecuteNonQueryAsync(cancellationToken);

        return affectedRows == 1;
    }

    // 6. PARAMETER PRODUCT CODE
    private static void AddProductCode(
        SqlCommand command,
        string productCode)
    {
        command.Parameters
            .Add("@productCode", SqlDbType.NVarChar, 30)
            .Value = productCode.Trim().ToUpperInvariant();
    }

    // 7. PARAMETER PRODUCT
    private static void AddProductParameters(
        SqlCommand command,
        Product product)
    {
        AddProductCode(command, product.ProductCode);

        command.Parameters
            .Add("@name", SqlDbType.NVarChar, 120)
            .Value = product.Name;

        command.Parameters
            .Add("@stock", SqlDbType.Int)
            .Value = product.Stock;

        command.Parameters
            .Add("@category", SqlDbType.NVarChar, 80)
            .Value = product.Category;

        var price = command.Parameters.Add(
            "@price", SqlDbType.Decimal);

        price.Precision = 18;
        price.Scale = 2;
        price.Value = product.Price;
    }

    // 8. SQL ROW -> PRODUCT MODEL
    private static Product ReadProduct(SqlDataReader reader)
    {
        return new Product(
            reader.GetString(0),
            reader.GetString(1),
            reader.GetDecimal(2),
            reader.GetInt32(3),
            reader.GetString(4)
        );
    }
}