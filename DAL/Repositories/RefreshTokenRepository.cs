#nullable enable
using System.Threading.Tasks;
using DAL.Interfaces;
using FirstCateringAuthenticationApi.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DAL.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthenticationContext _context;

        public RefreshTokenRepository(AuthenticationContext context)
        {
            _context = context;
        }

        public ValueTask<RefreshToken?> Get(string id)
        {
            return _context.RefreshTokens.FindAsync(id);
        }

        public ValueTask<EntityEntry<RefreshToken>> Create(RefreshToken token)
        {
            return _context.RefreshTokens.AddAsync(token);
        }

        public Task<int> Save()
        {
            return _context.SaveChangesAsync();
        }
    }
}