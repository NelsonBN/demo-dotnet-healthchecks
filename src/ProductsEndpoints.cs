using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;

namespace Demo.Api;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/products", async (IMongoCollection<Product> collection, CancellationToken cancellationToken = default) =>
        {
            var cursor = await collection.FindAsync(
                f => true,
                cancellationToken: cancellationToken);

            var products = cursor
                .ToEnumerable(cancellationToken)
                .Select(s => new ProductDto(s.Name, s.Price));

            return Results.Ok(products);
        });

        endpoints.MapPost("/products", async (ProductDto request, IMongoCollection<Product> collection, CancellationToken cancellationToken = default) =>
        {
            await collection.InsertOneAsync(
                document: new Product
                {
                    Name = request.Name,
                    Price = request.Price
                },
                options: null,
                cancellationToken: cancellationToken);

            return Results.NoContent();
        });
    }
}
