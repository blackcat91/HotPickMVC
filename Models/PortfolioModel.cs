using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace HotPickMVC.Models
{
    public class PortfolioModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public List<StockModel>? Stocks { get; set; } = new List<StockModel>();


        public string UserId { get; set; }
        public bool IsPublic { get; set; } = false;

        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;

        public double? HeatScore => Stocks.Any() ? Math.Round(Stocks!.Average(s => s.overall), 2): 0;
    }
}
