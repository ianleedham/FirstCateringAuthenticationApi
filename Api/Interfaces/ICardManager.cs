using FirstCateringAuthenticationApi.Classes;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FirstCateringAuthenticationApi.Interfaces
{
    public interface ICardManager
    {
        Task<IdentityResult> CreateAsync(IdentityCard card);

        Task<IdentityResult> CreateAsync(IdentityCard card, string password);

        Task<IdentityCard> FindByIdAsync(string userId);

        Task<IdentityCard> GetUserAsync(ClaimsPrincipal principal);

    }
}
