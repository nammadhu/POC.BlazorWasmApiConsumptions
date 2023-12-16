using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;

namespace CosmosDb
    {
    public interface ICosmosService
        {
        Task TestCreateDbContainerAddDataRetrieveFullFlow();
        Task<IEnumerable<Product>> RetrieveAllProductsAsync();
        Task<IEnumerable<Product>> RetrieveAllProductsWithConditionAsync(string? name = null, string? description = null);
        }

    public class CosmosService : ICosmosService
        {
        private readonly CosmosClient _client;
        private Container container
            {
            get => _client.GetDatabase("cosmicworks").GetContainer("products");
            }
        public CosmosService(IConfiguration configuration)
            {
            var cosmosDb = configuration.GetSection("CosmosDb");
            _client = new(accountEndpoint: cosmosDb.GetSection("AccountEndPoint").Value,
    authKeyOrResourceToken: cosmosDb.GetSection("AuthKeyOrResourceToken").Value);
            }

        public async Task TestCreateDbContainerAddDataRetrieveFullFlow()
            {

            // <new_database> 
            // Database reference with creation if it does not already exist
            Database database = await _client.CreateDatabaseIfNotExistsAsync(
                id: "cosmicworks"
            );

            Console.WriteLine($"New database:\t{database.Id}");
            // </new_database>

            // <new_container> 
            // Container reference with creation if it does not already exist
            Container container = await database.CreateContainerIfNotExistsAsync(
                id: "products01",
                partitionKeyPath: "/categoryid",
                throughput: 400
            );

            Console.WriteLine($"New container:\t{container.Id}");
            // </new_container>

            // <new_item> 
            // Create new object and upsert (create or replace) to container

            Product newItem = new Product(id: "baaa4d2d-5ebe-45fb-9a5c-d06876f408e0", categoryid: "61dba35b-4f02-45c5-b648-c6badc0cbd79", categoryName: "Components, Road Frames", sku: "FR-R72R-60", name: "ML Road Frame - Red, 60", description: "The product called ML Road Frame - Red, 60", price: 594.83000000000004m);

            /*
            Product createdItem = await container.CreateItemAsync<Product>(
                item: newItem,
                partitionKey: new PartitionKey("61dba35b-4f02-45c5-b648-c6badc0cbd79")
            );
           
            Console.WriteLine($"Created item:\t{createdItem.id}\t[{createdItem.categoryName}]"); 
            */
            // </new_item>

            Product updatedItem = new Product(id: "baaa4d2d-5ebe-45fb-9a5c-d06876f408e0", categoryid: "61dba35b-4f02-45c5-b648-c6badc0cbd79", categoryName: "Components, Road Frames", sku: "FR-R72R-60", name: "ML Road Frame - Red, 60", description: "The product called ML Road Frame - Red, 60" + DateTime.Now.ToString(), price: 594.83000000000004m);

            Product createdItem = await container.UpsertItemAsync<Product>(
                item: updatedItem,
                partitionKey: new PartitionKey("61dba35b-4f02-45c5-b648-c6badc0cbd79")
            );

            Console.WriteLine($"Updated item:\t{createdItem.id}\t[{createdItem.categoryName}]");

            // <read_item> 
            // Point read item from container using the id and partitionKey
            Product readItem = await container.ReadItemAsync<Product>(
                id: "baaa4d2d-5ebe-45fb-9a5c-d06876f408e0",
                partitionKey: new PartitionKey("61dba35b-4f02-45c5-b648-c6badc0cbd79")
            );
            // </read_item>

            // <query_items> 
            // Create query using a SQL string and parameters
            var query = new QueryDefinition(
                query: "SELECT * FROM products p WHERE p.categoryid = @categoryid"
            )
                .WithParameter("@categoryid", "61dba35b-4f02-45c5-b648-c6badc0cbd79");

            using FeedIterator<Product> feed = container.GetItemQueryIterator<Product>(
                queryDefinition: query
            );

            while (feed.HasMoreResults)
                {
                FeedResponse<Product> response = await feed.ReadNextAsync();
                foreach (Product item in response)
                    {
                    Console.WriteLine($"Found item:\t{item.name}");
                    }
                }
            }
        public async Task<IEnumerable<Product>> RetrieveAllProductsAsync()
            {
            var queryable = container.GetItemLinqQueryable<Product>();
            using FeedIterator<Product> feed = queryable
    //.Where(p => p.price < 2000m)
    //.OrderByDescending(p => p.price)
    .ToFeedIterator();

            List<Product> results = [];
            while (feed.HasMoreResults)
                {
                var response = await feed.ReadNextAsync();
                foreach (Product item in response)
                    {
                    results.Add(item);
                    }
                }
            return results;
            }
        public async Task<IEnumerable<Product>> RetrieveAllProductsWithConditionAsync(string? name = null, string? description = null)
            {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description))
                return await RetrieveAllProductsAsync();

            string sql = """
SELECT
    p.id,
    p.categoryId,
    p.categoryName,
    p.sku,
    p.name,
    p.description,
    p.price,
    p.tags
FROM products p
JOIN t IN p.tags
WHERE t.name = @tagFilter
""";
            var query = new QueryDefinition(query: sql).WithParameter("@tagFilter", "Tag-75");

            using FeedIterator<Product> feed = container.GetItemQueryIterator<Product>(queryDefinition: query);
            List<Product> results = [];

            while (feed.HasMoreResults)
                {
                FeedResponse<Product> response = await feed.ReadNextAsync();
                foreach (Product item in response)
                    {
                    results.Add(item);
                    }
                }

            return results;
            }


        }
    }
