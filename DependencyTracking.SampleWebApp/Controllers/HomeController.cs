using DependencyTracking.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Web.Http;

namespace DependencyTracking.SampleWebApp.Controllers
{
    [Route("api/test")]
    public class HomeController : ApiController
    {
        private readonly MongoClientFactory _mongoClientFactory;
        private readonly Http.HttpClientFactory _httpClientFactory;

        public HomeController(MongoClientFactory clientFactory, Http.HttpClientFactory httpClientFactory)
        {
            _mongoClientFactory = clientFactory;
            _httpClientFactory = httpClientFactory;
        }


        // GET api/test
        [HttpGet]
        public async Task<IHttpActionResult> Get()
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
