using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotPickMVC.Models
{
    public class StockModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; init; }

        public string ticker { get; set; }
        public string company { get; set; }

        public double momentum { get; set; }
        public double value { get; set; }


        [Column(TypeName = "decimal(18, 2)")]
        public double overall { get; set; } 
    }
}
