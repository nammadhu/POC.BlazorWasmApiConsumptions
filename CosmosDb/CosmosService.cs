using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
        public string _databaseName = "cosmicworks";
        public string _containerName = "products01";
        public string  _partitionKeyPath= "/categoryid";
        private Container Container
            {
            get => _client.GetDatabase(_databaseName).GetContainer(_containerName);
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
                id: _databaseName
            );

            Console.WriteLine($"New database:\t{database.Id}");
            // </new_database>

            // <new_container> 
            // Container reference with creation if it does not already exist
            Container container = await database.CreateContainerIfNotExistsAsync(
                id: _containerName,
                partitionKeyPath: _partitionKeyPath,
                throughput: 400
            );

            Console.WriteLine($"New container:\t{container.Id}");
            // </new_container>

            // <new_item> 
            // Create new object and upsert (create or replace) to container

            Product newItem = new Product(id: "baaa4d2d-5ebe-45fb-9a5c-d06876f408e0", categoryid: "61dba35b-4f02-45c5-b648-c6badc0cbd79", categoryName: "Components, Road Frames", sku: "FR-R72R-60", name: "ML Road Frame - Red, 60", description: "The product called ML Road Frame - Red, 60", price: 594.83000000000004m);

            if (Debugger.IsAttached)
                {
                Product createdItem = await container.CreateItemAsync<Product>(
                    item: newItem,
                    partitionKey: new PartitionKey("61dba35b-4f02-45c5-b648-c6badc0cbd79")
                );

                Console.WriteLine($"Created item:\t{createdItem.id}\t[{createdItem.categoryName}]");
                }
            // </new_item>

            Product toUpdateItem = new Product(id: "baaa4d2d-5ebe-45fb-9a5c-d06876f408e0", categoryid: "61dba35b-4f02-45c5-b648-c6badc0cbd79", categoryName: "Components, Road Frames", sku: "FR-R72R-60", name: "ML Road Frame - Red, 60", description: "The product called ML Road Frame - Red, 60" + DateTime.Now.ToString(), price: 594.83000000000004m);

            Product updatedItem = await container.UpsertItemAsync<Product>(
                item: toUpdateItem,
                partitionKey: new PartitionKey("61dba35b-4f02-45c5-b648-c6badc0cbd79")
            );

            Console.WriteLine($"Updated item:\t{updatedItem.id}\t[{updatedItem.categoryName}]");

            // <read_item> 
            // Point read item from container using the id and partitionKey
            var readItem = await container.ReadItemAsync<Product>(
                id: "baaa4d2d-5ebe-45fb-9a5c-d06876f408e0",
                partitionKey: new PartitionKey("61dba35b-4f02-45c5-b648-c6badc0cbd79")
            );
            if (readItem == null)
                Console.WriteLine("Item not foind");
            else
                Console.WriteLine("Item found...." + readItem);
            // </read_item>

            // <query_items> 
            // Create query using a SQL string and parameters
            var query = new QueryDefinition(
                query: $"SELECT * FROM {_containerName} p WHERE p.categoryid = @categoryid"
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
            var queryable = Container.GetItemLinqQueryable<Product>();
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

            string sql = $"""
SELECT
    p.id,
    p.categoryid,
    p.categoryName,
    p.sku,
    p.name,
    p.description,
    p.price
FROM {_containerName} p
WHERE contains( p.name ,@nameFilter) 
""";
            var query = new QueryDefinition(query: sql).WithParameter("@nameFilter", name);

            using FeedIterator<Product> feed = Container.GetItemQueryIterator<Product>(queryDefinition: query);
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
