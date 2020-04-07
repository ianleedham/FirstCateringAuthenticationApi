using DAL.Models;
using FirstCateringAuthenticationApi.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AuthenticationContext : IdentityDbContext<IdentityCard>
    {

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {

        }
    }
}
