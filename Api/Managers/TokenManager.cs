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
    /// <summary>
    /// For managing json web and refresh tokens
    /// </summary>
    public class TokenManager : ITokenManager
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// The constructor with injected dependancies
        /// </summary>
        /// <param name="refreshTokenRepository"></param>
        /// <param name="configuration"></param>
        public TokenManager(IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }
        
        /// <summary>
        /// To save a given refresh token using the token repository
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>Task/bool</returns>
        public async Task<bool> SaveRefreshToken(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.Create(refreshToken);
            var result = await _refreshTokenRepository.Save();
            return result > 0;
        }

        /// <summary>
        /// To create a new jwt
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>string of the jwt</returns>
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

        /// <summary>
        /// To create a refresh token for a card 
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>The RefreshToken object</returns>
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

        /// <summary>
        /// To invalidate a refresh token
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>Task</returns>
        public async Task InvalidateRefreshToken(string cardNumber)
        {
            RefreshToken token = await _refreshTokenRepository.GetByCardnumber(cardNumber);
            if (token != null)
            {
                token.Revoked = true;
                _refreshTokenRepository.Update(token);
                await _refreshTokenRepository.Save();
            }
        }

        /// <summary>
        /// To reset a refresh tokens expiry date/time
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>Task which returns the RefreshToken object</returns>
        public async Task<RefreshToken> ResetRefreshToken(RefreshToken refreshToken)
        {
            RefreshToken token = await _refreshTokenRepository.Get(refreshToken.Token);
            if (token != null)
            {
                token.Expires = DateTime.Now.AddMinutes(5);
                _refreshTokenRepository.Update(token);
                await _refreshTokenRepository.Save();
            };
            return token;
        }
    }
}