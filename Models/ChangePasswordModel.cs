using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotPickMVC.Models
{
    public class ChangePasswordModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        
        [DisplayName("Current Password")]
        [Required]
        public string CurrentPassword { get; set; } 
        
        [DisplayName("New Password")]
        [Required]
        public string NewPassword { get; set; } 
        
        [DisplayName("Confirm Password")]
        [Required]
        public string ConfirmNewPassword { get; set; } 


    }
}
