using Microsoft.AspNetCore.Identity;

namespace FirstCateringAuthenticationApi.Classes
{
    public class IdentityCard : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
