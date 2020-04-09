using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Models;

namespace FirstCateringAuthenticationApi.Interfaces
{
    /// <summary>
    /// The card manager for creating find and checking cards
    /// </summary>
    public interface ICardManager
    {
        /// <summary>
        /// For creating cards with a pin
        /// </summary>
        /// <param name="card"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        Task<IdentityResult> CreateAsync(IdentityCard card, string pin);

        /// <summary>
        /// For finding a card by its card number
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>The identity card async</returns>
        Task<IdentityCard> FindByIdAsync(string cardNumber);

        /// <summary>
        /// For checking if a given pin is correct for a given card
        /// </summary>
        /// <param name="card"></param>
        /// <param name="pin"></param>
        /// <returns>bool async</returns>
        Task<bool> CheckPasswordAsync(IdentityCard card, string pin);
    }
}
