using Microsoft.AspNetCore.Identity;

namespace DAL.Models
{
    public class IdentityCard : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
