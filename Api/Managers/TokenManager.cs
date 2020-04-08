using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using FirstCateringAuthenticationApi.Interfaces;
using FirstCateringAuthenticationApi.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FirstCateringAuthenticationApi.Managers
{
    public class TokenManager : ITokenManager
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        public TokenManager(IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }
        
        public async Task<bool> SaveRefreshToken(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.Create(refreshToken);
            var result = await _refreshTokenRepository.Save();
            return result > 0;
        }

        public string CreateJwt(string cardNumber)
        {
            var tokenHandeler = new JwtSecurityTokenHandler { TokenLifetimeInMinutes = 2 };

            byte[] key = Encoding.ASCII.GetBytes(_configuration["Jwt:IssuerKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, cardNumber)
                }),
                Expires = DateTime.Now.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            SecurityToken token = tokenHandeler.CreateToken(tokenDescriptor);

            return tokenHandeler.WriteToken(token);
        }

        public RefreshToken CreateRefreshToken(string cardNumber)
        {
            var refreshToken = new RefreshToken()
            {
                CardNumber = cardNumber,
                Token = Guid.NewGuid().ToString(),
                Revoked = false,
                Expires = DateTime.Now.AddMinutes(5),
            };
            return refreshToken;
        }

        public async Task InvalidateRefreshToken(string cardNumber)
        {
            RefreshToken token = await _refreshTokenRepository.Get(cardNumber);
            if (token != null)
            {
                token.Revoked = true;
                await _refreshTokenRepository.Save();
            }
        }
    }
}