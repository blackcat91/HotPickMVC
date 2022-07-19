using MongoDB.Bson.Serialization.Attributes;

namespace HotPickMVC.Models
{
    public class Jwt
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Token { get; set; }

        public DateTime Expiry { get; set; }
    }
}
