namespace CosmosDb;

public record Product(
    string id,
    string categoryid,
    string categoryName,
    string sku,
    string name,
    string description,
    decimal price
);