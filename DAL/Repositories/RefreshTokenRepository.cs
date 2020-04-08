#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;
using FirstCateringAuthenticationApi.Model;
using Microsoft.EntityFrameworkCore;
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

        public async Task<RefreshToken> Get(string tokenString)
        {
            return await _context.RefreshTokens.FindAsync(tokenString);
        }
        
        public Task<RefreshToken> GetByCardnumber(string cardNumber)
        {
            return _context.RefreshTokens.Where(x => x.Revoked == false &&
                                                     x.CardNumber == cardNumber && x.Expires > DateTime.Now)
                .FirstOrDefaultAsync();
        }

        public ValueTask<EntityEntry<RefreshToken>> Create(RefreshToken token)
        {
            return _context.RefreshTokens.AddAsync(token);
        }

        public void Update(RefreshToken token)
        {
            _context.Update(token);
        }

        public Task<int> Save()
        {
            return _context.SaveChangesAsync();
        }
    }
}