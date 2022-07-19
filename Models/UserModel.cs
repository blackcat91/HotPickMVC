using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace HotPickMVC.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; init; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }

        public Jwt Jwt { get; set; } = new Jwt();
        [Required]
        public string? Password { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;


        public List<string>? Portfolios { get; set; } = new List<string> { };

        public string? Error { get; set; } = string.Empty;

    }
}
