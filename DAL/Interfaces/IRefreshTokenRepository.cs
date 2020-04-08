#nullable enable
using FirstCateringAuthenticationApi.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> Get(string tokenString);

        Task<RefreshToken?> GetByCardnumber(string cardNumber);

        void Update(RefreshToken token);

        ValueTask<EntityEntry<RefreshToken>> Create(RefreshToken token);

        public Task<int> Save();
    }
}