using HotPickMVC.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace HotPickMVC.Data
{
    public class MongoDBAccess
    {
        
        private readonly IConfiguration _configuration;

        public MongoDBAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public const string DatabaseName = "hotpick";
     

        public IMongoCollection<T> ConnectToMongo<T>(in string collection)
        {
            var client = new MongoClient(_configuration.GetConnectionString("Default"));
            var db = client.GetDatabase(DatabaseName);
            return db.GetCollection<T>(collection);
        }

        public Jwt? VerifyJwt()
        {
            try
            {
                var file = System.IO.File.ReadAllText(Path.GetTempPath() + "token.json");
                Jwt? jwt = JsonConvert.DeserializeObject<Jwt>(file);
                Console.WriteLine(jwt!.Token);
                if (jwt.Expiry > DateTime.UtcNow)
                {
                   
                    return jwt;
                }
                else
                {
                    File.Delete(Path.GetTempPath() + "token.json");
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

       
    }
}
