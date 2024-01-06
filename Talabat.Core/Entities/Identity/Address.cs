using System.Text.Json.Serialization;

namespace Talabat.Core.Entities.Identity
{
    public class Address
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string AppUserId { get; set; } // Foreign Key :Users
        public string Country { get; set; }
        //[JsonIgnore]
        public AppUser User { get; set; }
    }
}