using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DependencyTracking.MongoDb.SampleWebApp.Controllers
{
    [Route("api/test")]
    public class HomeController : Controller
    {
        private readonly MongoClientFactory _clientFactory;

        public HomeController(MongoClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }


        // GET api/test
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            const string connectionString = "mongodb://localhost:27017";

            var client = _clientFactory.GetClient(connectionString);
            var db = client.GetDatabase("TestDb");
            var collection = db.GetCollection<BsonDocument>("CollectionA");

            for (var i = 0; i < 5; i++)
            {
                await collection.FindAsync(new JsonFilterDefinition<BsonDocument>("{'A':123}"));
            }

            return Ok();
        }
    }
}
