using System.Threading.Tasks;
using FirstCateringAuthenticationApi.Model;

namespace FirstCateringAuthenticationApi.Interfaces
{
    /// <summary>
    /// The manager for json web and refresh tokens
    /// </summary>
    public interface ITokenManager
    {
        /// <summary>
        /// To save a refresh token to the database
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>bool as to if it was successful</returns>
        Task<bool> SaveRefreshToken(RefreshToken refreshToken);
        
        /// <summary>
        /// To create a new jwt
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>string of the jwt</returns>
        string CreateJwt(string cardNumber);
        
        /// <summary>
        /// Create a refresh token for a card
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>A RefreshToken</returns>
        RefreshToken CreateRefreshToken(string cardNumber);
        
        /// <summary>
        /// To invalidate a refresh token
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>Task</returns>
        Task InvalidateRefreshToken(string cardNumber);
        
        /// <summary>
        /// To reset a refresh tokens expiry date/time
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>The refesh token async</returns>
        Task<RefreshToken> ResetRefreshToken(RefreshToken refreshToken);
    }
}