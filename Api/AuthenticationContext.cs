using FirstCateringAuthenticationApi.Classes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FirstCateringAuthenticationApi
{
    public class AuthenticationContext : IdentityDbContext<IdentityCard>
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {

        }
    }
}
