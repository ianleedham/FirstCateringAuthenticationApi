using System.Threading.Tasks;
using FirstCateringAuthenticationApi.Model;

namespace FirstCateringAuthenticationApi.Interfaces
{
    public interface ITokenManager
    {
        Task<bool> SaveRefreshToken(RefreshToken refreshToken);
        
        string CreateJwt(string cardNumber);
        
        RefreshToken CreateRefreshToken(string cardNumber);
        
        Task InvalidateRefreshToken(string cardNumber);
    }
}