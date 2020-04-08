using FirstCateringAuthenticationApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Interfaces;
using DAL.Models;
using FirstCateringAuthenticationApi.DataTransferObjects;
using FirstCateringAuthenticationApi.Model;
using Microsoft.AspNetCore.Authorization;

namespace FirstCateringAuthenticationApi.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ICardManager _cardManager;
        private readonly IMapper _mapper;
        private readonly ITokenManager _tokenManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthenticationController(ICardManager userManager, IMapper mapper, ITokenManager tokenManager, IRefreshTokenRepository refreshTokenRepository)
        {
            _cardManager = userManager;
            _mapper = mapper;
            _tokenManager = tokenManager;
            _refreshTokenRepository = refreshTokenRepository;
        }
        
        [Authorize]
        [HttpPost("tap")]
        public async Task<IActionResult> Tap([FromBody] TapDto dto)
        {
            var card = _cardManager.FindByIdAsync(dto.CardNumber);
            if (card == null)
            {
                return NotFound("please register your card");
            }
            await _tokenManager.InvalidateRefreshToken(dto.CardNumber);
            return Ok("logged out successfully");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginParametersDto loginParametersDto )
        {
            IdentityCard card = await _cardManager.FindByIdAsync(loginParametersDto.CardNumber);
            
            // check null card
            if (card == null)
            {
                return NotFound("please register your card");
            }

            // check password
            var passwordCorrect = await _cardManager.CheckPasswordAsync(card, loginParametersDto.Pin);
            if (!passwordCorrect) return Unauthorized("pin incorrect");
            
            var refreshToken = _tokenManager.CreateRefreshToken(loginParametersDto.CardNumber);
            var cardDto = _mapper.Map<CardDto>(card);
            cardDto.Bearer = _tokenManager.CreateJwt(loginParametersDto.CardNumber);
            cardDto.RefreshToken = null;
            if (await _tokenManager.SaveRefreshToken(refreshToken))
            {
                cardDto.RefreshToken = refreshToken.Token;
            }
            return Ok(cardDto);

        }
        
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            RefreshToken refreshToken = await _refreshTokenRepository.Get(dto.Token);
            if (refreshToken == null || refreshToken.Revoked || refreshToken.Expires <= DateTime.Now)
            {
                return Unauthorized();
            }

            var card = await _cardManager.FindByIdAsync(refreshToken.CardNumber);
            refreshToken.Expires = DateTime.Now.AddMinutes(5);
            var cardDto = _mapper.Map<CardDto>(card);
            cardDto.Bearer = _tokenManager.CreateJwt(refreshToken.CardNumber);
            cardDto.RefreshToken = null;
            if (await _tokenManager.SaveRefreshToken(refreshToken))
            {
                cardDto.RefreshToken = refreshToken.Token;
            }
            return Ok(cardDto);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CardRegistrationDto dto)
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

            // use auto mapper to create IdentityCard from the dto
            var card = _mapper.Map<IdentityCard>(dto);

            IdentityResult identityResult = await _cardManager.CreateAsync(card, dto.Pin);

            if (identityResult.Succeeded)
            {
                return Ok(await _cardManager.FindByIdAsync(dto.CardNumber));
            }

            return BadRequest(identityResult.Errors);
        }
    }
}