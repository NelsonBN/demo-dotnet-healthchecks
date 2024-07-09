using MongoDB.Bson;

namespace Demo.Api;

public class Product
{
    public ObjectId Id { get; set; }
    public required string Name { get; set; } = default!;
    public required double Price { get; set; }
}

public record ProductDto(string Name, double Price);
