#nullable enable
using FirstCateringAuthenticationApi.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRefreshTokenRepository
    {
        ValueTask<RefreshToken?> Get(string id);

        ValueTask<EntityEntry<RefreshToken>> Create(RefreshToken token);

        public Task<int> Save();
    }
}