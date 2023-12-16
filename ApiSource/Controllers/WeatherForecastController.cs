using Microsoft.AspNetCore.Mvc;

namespace ApiSource.Controllers
    {
    [ApiController]
    //[Route("[controller]")]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
        {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly CosmosDb.ICosmosService _cosmosService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, CosmosDb.ICosmosService cosmosService)
            {
            _logger = logger;
            _cosmosService = cosmosService;
            }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
            {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            .ToArray();
            }

        [HttpGet(Name = "CosmosDbTest")]
        public async Task<IEnumerable<WeatherForecast>> CosmosDbTest(string name)
            {
            /*
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine($"Starting {nameof(_cosmosService.TestCreateDbContainerAddDataRetrieveFullFlow)}");
            await _cosmosService.TestCreateDbContainerAddDataRetrieveFullFlow();
            Console.WriteLine($"Completed {nameof(_cosmosService.TestCreateDbContainerAddDataRetrieveFullFlow)}");
            Console.WriteLine("***********************");


            Console.WriteLine($"Starting {nameof(_cosmosService.RetrieveAllProductsAsync)}");
            var results = await _cosmosService.RetrieveAllProductsAsync();
            foreach (var item in results)
                {
                Console.WriteLine($"Found item:\t{item.name},{item}");
                }
            Console.WriteLine($"Completed {nameof(_cosmosService.RetrieveAllProductsAsync)}");
            */
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Starting {nameof(_cosmosService.RetrieveAllProductsWithConditionAsync)} with name:{name}");
            var results = await _cosmosService.RetrieveAllProductsWithConditionAsync(name);
            foreach (var item in results)
                {
                Console.WriteLine($"Found item:\t{item.name},{item}");
                }
            Console.WriteLine($"Coompleted {nameof(_cosmosService.RetrieveAllProductsWithConditionAsync)} with name:{name}");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            .ToArray();
            }
        }
    }
