using System.Threading.Tasks;
using DependencyTracking.Http;
using DependencyTracking.MongoDb;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DependencyTracking.SampleCoreWebApp.Controllers
{
    [Route("api/test")]
    public class HomeController : Controller
    {
        private readonly MongoClientFactory _mongoClientFactory;
        private readonly HttpClientFactory _httpClientFactory;

        public HomeController(MongoClientFactory clientFactory, HttpClientFactory httpClientFactory)
        {
            _mongoClientFactory = clientFactory;
            _httpClientFactory = httpClientFactory;
        }


        // GET api/test
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await MongoExample();
            await HttpExample();

            return Ok();
        }

        private async Task MongoExample()
        {
            var connectionString = "mongodb://localhost:27017";
            var client = _mongoClientFactory.GetClient(connectionString);
            var db = client.GetDatabase("TestDb");
            var collection = db.GetCollection<BsonDocument>("CollectionA");
            for (int i = 0; i < 5; i++)
            {
                await collection.FindAsync(new JsonFilterDefinition<BsonDocument>("{'A':123}"));
            }
        }

        private async Task HttpExample()
        {
            var targetUrl = "https://google.com.ua";
            var client = _httpClientFactory.GetClient();
            for (int i = 0; i < 5; i++)
            {
                await client.GetAsync(targetUrl);
            }
        }
    }
}
