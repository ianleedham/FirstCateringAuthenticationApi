using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Models;

namespace FirstCateringAuthenticationApi.Interfaces
{
    public interface ICardManager
    {
        Task<IdentityResult> CreateAsync(IdentityCard card, string password);

        Task<IdentityCard> FindByIdAsync(string userId);

        Task<bool> CheckPasswordAsync(IdentityCard user, string password);
    }
}
