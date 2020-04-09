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
using Microsoft.AspNetCore.Http;

namespace FirstCateringAuthenticationApi.Controllers
{
    /// <summary>
    /// Class for autheticating, getting or refreshing jwt and registering
    /// </summary>
    [ApiController]
    [Route("authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ICardManager _cardManager;
        private readonly IMapper _mapper;
        private readonly ITokenManager _tokenManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        /// <summary>
        /// AuthenticationController's constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="mapper"></param>
        /// <param name="tokenManager"></param>
        /// <param name="refreshTokenRepository"></param>
        public AuthenticationController(ICardManager userManager, IMapper mapper, ITokenManager tokenManager, IRefreshTokenRepository refreshTokenRepository)
        {
            _cardManager = userManager;
            _mapper = mapper;
            _tokenManager = tokenManager;
            _refreshTokenRepository = refreshTokenRepository;
        }
        
        /// <summary>
        /// When a user taps their card it can call this to find out what if anything the client needs to do
        /// </summary>
        /// <param name="dto">contains the card number</param>
        /// <returns>logged out or unauthorised</returns>
        /// <response code="401"> Unauthorized if they sent a expired of null jwt</response>
        /// <response code="404"> NotFound("please register your card") if the card was not found</response>
        /// <response code="200"> Ok("logged out successfully") if they sent a valid jwt and a registered card number</response>
        [Authorize]
        [HttpPost("tap")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
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

        /// <summary>
        /// The main login method
        /// </summary>
        /// <param name="loginParametersDto">includes card number and pin</param>
        /// <returns>cardDto</returns>
        /// <response code="404">NotFound if the card was not found</response>
        /// <response code="401">Unauthorized if the pin was incorrect</response>
        /// <response code="200">if all ok</response>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
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
        
        /// <summary>
        /// The end point for getting a new jwt using the refresh token
        /// </summary>
        /// <param name="dto">includes the refresh token</param>
        /// <returns>cardDto</returns>
        ///<response code="401">unauthorised if the token is null, revoked or expired</response>
        ///<response code="200">Ok(cardDto) if all ok</response>
        
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            RefreshToken refreshToken = await _refreshTokenRepository.Get(dto.Token);
            if (refreshToken == null || refreshToken.Revoked || refreshToken.Expires <= DateTime.Now)
            {
                return Unauthorized();
            }

            var card = await _cardManager.FindByIdAsync(refreshToken.CardNumber);
            var cardDto = _mapper.Map<CardDto>(card);
            cardDto.Bearer = _tokenManager.CreateJwt(refreshToken.CardNumber);
            cardDto.RefreshToken = null;
            refreshToken = await _tokenManager.ResetRefreshToken(refreshToken);
            cardDto.RefreshToken = refreshToken.Token;
            
            return Ok(cardDto);
        }
        
        /// <summary>
        /// The end point for registering new cards 
        /// </summary>
        /// <param name="dto">includes all the card and card holders details needed to register</param>
        /// <returns>"Registered successfully"</returns>
        /// <response code="400">Bad request with Validation errors if the dto is invalid</response>
        /// <response code="400">BadRequest if the card is already registered</response>
        /// <response code="200">Ok("Registered successfully") if all ok</response>
        /// <response code="400">BadRequest with identity errors if something still went wrong in registering</response>

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
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
                return Ok("Registered successfully");
            }

            return BadRequest(identityResult.Errors);
        }
    }
}