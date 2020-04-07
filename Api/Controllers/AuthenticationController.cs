using FirstCateringAuthenticationApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Interfaces;
using DAL.Models;
using FirstCateringAuthenticationApi.DataTransferObjects;
using FirstCateringAuthenticationApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FirstCateringAuthenticationApi.Controllers
{
    [ApiController]
    [Route("authentication")]
    [AllowAnonymous()]
    public class AuthenticationController : ControllerBase
    {
        private readonly ICardManager _cardManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthenticationController(ICardManager userManager, IMapper mapper, IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
        {
            _cardManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Tap(string cardNumber)
        {
            await InvalidateRefreshToken(cardNumber);
            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> RefreshToken(string token)
        {
            RefreshToken refreshToken = await _refreshTokenRepository.Get(token);
            var card = _cardManager.FindByIdAsync(refreshToken.CardNumber);
            if (refreshToken == null || refreshToken.Revoked || refreshToken.Expires <= DateTime.Now)
            {
                return Unauthorized();
            }
            var cardDto = _mapper.Map<CardDto>(card);
            cardDto.Bearer = CreateJwt(refreshToken.CardNumber);
            cardDto.RefreshToken = null;
            if (await SaveRefreshToken(refreshToken))
            {
                cardDto.RefreshToken = refreshToken.Token;
            }
            return Ok(cardDto);
        }

        [HttpPost("tap")]
        public async Task<IActionResult> TapWithPin(string cardNumber, string pin)
        {
            IdentityCard card = await _cardManager.FindByIdAsync(cardNumber);
            
            // check null card
            if (card == null)
            {
                return NotFound("please register your card");
            }

            // check password
            var passwordCorrect = await _cardManager.CheckPasswordAsync(card, pin);
            if (!passwordCorrect) return Unauthorized("pin incorrect");
            
            var refreshToken = CreateRefreshToken(cardNumber);
            var cardDto = _mapper.Map<CardDto>(card);
            cardDto.Bearer = CreateJwt(cardNumber);
            cardDto.RefreshToken = null;
            if (await SaveRefreshToken(refreshToken))
            {
                cardDto.RefreshToken = refreshToken.Token;
            }
            return Ok(cardDto);

        }

        private async Task<bool> SaveRefreshToken(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.Create(refreshToken);
            var result = await _refreshTokenRepository.Save();
            return result > 0;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(CardRegistrationDto dto)
        {
            // check if card is already registered
            IdentityUser savedCard = await _cardManager.FindByIdAsync(dto.CardNumber);
            if (savedCard != null)
            {
                return BadRequest("Card already registered");
            }

            // check model is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }

            // use auto mapper to create IdentityCard from the model
            var card = new IdentityCard()
            {
                Id = dto.CardNumber,
                UserName = dto.EmployeeId,
                PhoneNumber = dto.PhoneNumer,
                Email = dto.Email
            };

            IdentityResult identityResult = await _cardManager.CreateAsync(card, dto.Pin);

            if (identityResult.Succeeded)
            {
                savedCard = await _cardManager.FindByIdAsync(dto.CardNumber);
                return Ok(savedCard);
            }
            else
            {
                return BadRequest(identityResult.Errors);
            }
        }

        private string CreateJwt(string cardNumber)
        {
            var tokenHandeler = new JwtSecurityTokenHandler { TokenLifetimeInMinutes = 2 };

            byte[] key = Encoding.ASCII.GetBytes("superSecretPassword");

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

        private RefreshToken CreateRefreshToken(string cardNumber)
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

        private async Task InvalidateRefreshToken(string cardNumber)
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