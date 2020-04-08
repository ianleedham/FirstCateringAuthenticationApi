using System;
using FirstCateringAuthenticationApi.Controllers;
using FirstCateringAuthenticationApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Interfaces;
using DAL.Models;
using FirstCateringAuthenticationApi.DataTransferObjects;
using FirstCateringAuthenticationApi.Model;
using Microsoft.Extensions.Configuration;
using UnitTests.Accessors;
using UnitTests.Builders;
using Xunit;

namespace UnitTests
{
    public class AuthenticationControllerTests
    {
        private Mock<ICardManager> _mockCardManager;
        private Mock<IMapper> _mockMapper;
        private Mock<ITokenManager> _mockTokenManager;
        private Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        
        private readonly IdentityCardBuilder _identityCardBuilder;
        private readonly CardRegistrationDtoBuilder _cardRegistrationDtoBuilder;

        public AuthenticationControllerTests()
        {
            _identityCardBuilder = new IdentityCardBuilder();
            _cardRegistrationDtoBuilder = new CardRegistrationDtoBuilder();
        }

        [Fact]
        public async Task UserTaps_WithLoggedinUser_InvalidatesRefreshTokenReturnsOk()
        {
            // Arrange
            var subject = GetSubject();
            IdentityCard card = _identityCardBuilder.GenericIdentityCard().Build();
            _mockCardManager.Setup(x => x.FindByIdAsync(card.Id)).ReturnsAsync(card);
            
            // Act
            var result = await subject.Tap(card.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(OkResult), result.GetType());
            _mockTokenManager.Verify(x => x.InvalidateRefreshToken(card.Id));
        }
        
        [Fact]
        public async Task UserTaps_WithUnregisteredCard_ReturnsUnauthorised()
        {
            // Arrange
            var subject = GetSubject();
            IdentityCard card = _identityCardBuilder.GenericIdentityCard().Build();
            
            // Act
            var result = await subject.Tap(card.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(OkResult), result.GetType());
            _mockTokenManager.Verify(x => x.InvalidateRefreshToken(card.Id));
        }
        
        [Fact]
        public async Task ClientCallsLogin_WithUnregisteredCard_ReturnsNotfound()
        {
            // Arrange
            var subject = GetSubject();
            var cardNumber = "cutbgln213454hbv";
            var pin = "1234";
            _mockCardManager.Setup(x => x.FindByIdAsync(cardNumber)).ReturnsAsync((IdentityCard) null);
            var loginParameters = new LoginParametersDto();
            
            // Act
            var result = await subject.Login(loginParameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(NotFoundObjectResult), result.GetType());
            Assert.Equal("please register your card", (result as NotFoundObjectResult).Value);
        }

        [Fact]
        public async Task ClientSendsLoginInfo_WithRegisteredCardAndCorrectPin_LogsUserInAndReturnsCardDto()
        {
            // Arrange
            var subject = GetSubject();
            const string pin = "1234";
            const string jwt = "mock1jwt";
            var refreshTokenValue = "mock1refreshToken";
            var card = _identityCardBuilder.GenericIdentityCard().Build();
            card.Id = "cutbgln213454hbv";
            _mockCardManager.Setup(x => x.FindByIdAsync(card.Id)).ReturnsAsync(card);
            _mockCardManager.Setup(x => x.CheckPasswordAsync(card, pin)).ReturnsAsync(true);
            var loginParameters = new LoginParametersDto()
            {
                CardNumber = card.Id,
                Pin = pin
            };
            var cardDto = new CardDto()
            {
                CardNumber = card.Id,
                FullName = card.FullName,
            };
            var refreshToken = new RefreshToken()
            {
                Token = refreshTokenValue
            };
            _mockMapper.Setup(x => x.Map<CardDto>(card)).Returns(cardDto);
            _mockTokenManager.Setup(x => x.CreateJwt(card.Id)).Returns(jwt);
            _mockTokenManager.Setup(x => x.CreateRefreshToken(card.Id)).Returns(refreshToken);
            _mockTokenManager.Setup(x => x.SaveRefreshToken(refreshToken)).ReturnsAsync(true);
            // Act
            var result = await subject.Login(loginParameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(OkObjectResult), result.GetType());
            var okObject = result as OkObjectResult;
            Assert.Equal(typeof(CardDto), okObject.Value.GetType());
            var resultCardDto = (CardDto) okObject.Value;
            Assert.Equal(card.FullName, resultCardDto.FullName);
            Assert.Equal(card.Id, resultCardDto.CardNumber);
            Assert.Equal(jwt, resultCardDto.Bearer);
            Assert.Equal(refreshTokenValue, resultCardDto.RefreshToken);
        }
        
        [Fact]
        public async Task ClientSendsLoginInfo_WithRegisteredCardAndIncorrectPin_ReturnsUnauthorised()
        {
            // Arrange
            var subject = GetSubject();
            var pin = "1234";
            var card = _identityCardBuilder.GenericIdentityCard().Build();
            card.Id = "cutbgln213454hbv";
            _mockCardManager.Setup(x => x.FindByIdAsync(card.Id)).ReturnsAsync(card);
            _mockCardManager.Setup(x => x.CheckPasswordAsync(card, pin)).ReturnsAsync(false);
            var loginParameters = new LoginParametersDto()
            {
                CardNumber = card.Id,
                Pin = pin
            };
            
            // Act
            var result = await subject.Login(loginParameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(UnauthorizedObjectResult), result.GetType());
            var resultObject = result as UnauthorizedObjectResult;
            Assert.Equal("pin incorrect",resultObject.Value);
        }

        [Fact]
        public async Task UserRegisters_WithValidDetails_RegisteresUserAndReturnsRegisteredSuccessfully()
        {
            // Arrange
            var subject = GetSubject();
            IdentityCard identityCard = _identityCardBuilder.GenericIdentityCard().Build();
            CardRegistrationDto cardRegistrationDto = _cardRegistrationDtoBuilder.GenericRegistration().Build();
            identityCard.Id = cardRegistrationDto.CardNumber;
            _mockMapper.Setup(x => x.Map<IdentityCard>(cardRegistrationDto)).Returns(identityCard);

            var identityResult = new IdentityResultAccessor(true);
            _mockCardManager.Setup(x => x.CreateAsync(identityCard, cardRegistrationDto.Pin))
                .ReturnsAsync(identityResult);
            _mockCardManager.SetupSequence(x => x.FindByIdAsync(identityCard.Id))
                .ReturnsAsync((IdentityCard) null)
                .ReturnsAsync(identityCard);
            _mockCardManager.SetupSequence(x => x.FindByIdAsync(identityCard.Id))
                .ReturnsAsync((IdentityCard) null)
                .ReturnsAsync(identityCard);

            // Act
            var result = await subject.RegisterAsync(cardRegistrationDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(OkObjectResult), result.GetType());
            Assert.Equal(typeof(IdentityCard), ((OkObjectResult) result).Value.GetType());
        }

        [Fact]
        public async Task ClientRefreshToken_WithExpiredRefreshToken_ReturnsUnauthorized()
        {
            // Arrange
            var subject = GetSubject();
            const string refreshTokenString = "12345jhhjvhk567rgyuhjkln";
            var refreshToken = new RefreshToken()
            {
                Revoked = false,
                Expires = DateTime.Now.AddHours(-1)
            };
            _mockRefreshTokenRepository.Setup(x => x.Get(refreshTokenString)).ReturnsAsync(refreshToken);
            
            // Act
            
            var result = await subject.RefreshToken(refreshTokenString);

            // Assert
            Assert.Equal(typeof(UnauthorizedResult), result.GetType());
        }
        
        [Fact]
        public async Task ClientRefreshToken_WithRevokedRefreshToken_ReturnsUnauthorized()
        {
            // Arrange
            var subject = GetSubject();
            const string refreshTokenString = "12345jhhjvhk567rgyuhjkln";
            var refreshToken = new RefreshToken()
            {
                Revoked = true,
                Expires = DateTime.Now.AddDays(1)
            };
            _mockRefreshTokenRepository.Setup(x => x.Get(refreshTokenString)).ReturnsAsync(refreshToken);
            
            // Act
            
            var result = await subject.RefreshToken(refreshTokenString);

            // Assert
            Assert.Equal(typeof(UnauthorizedResult), result.GetType());
        }
        
        [Fact]
        public async Task ClientRefreshToken_WithValidRefreshToken_ReturnsOkWithCardDto()
        {
            // Arrange
            var subject = GetSubject();
            const string refreshTokenString = "12345jhhjvhk567rgyuhjkln";
            var identityCard = _identityCardBuilder.GenericIdentityCard().Build();
            string cardNumber = identityCard.Id;
            var refreshToken = new RefreshToken()
            {
                CardNumber = cardNumber,
                Revoked = false,
                Expires = DateTime.Now.AddDays(1)
            };
            _mockRefreshTokenRepository.Setup(x => x.Get(refreshTokenString)).ReturnsAsync(refreshToken);
            CardDto cardDto = new CardDto();
            _mockMapper.Setup(x => x.Map<CardDto>(identityCard)).Returns(cardDto);
            _mockCardManager.Setup(x => x.FindByIdAsync(cardNumber)).ReturnsAsync(identityCard);
            _mockTokenManager.Setup(x => x.SaveRefreshToken(refreshToken)).ReturnsAsync(true);
            
            // Act
            var result = await subject.RefreshToken(refreshTokenString);

            // Assert
            Assert.Equal(typeof(OkObjectResult), result.GetType());
            Assert.Equal(typeof(CardDto), ((OkObjectResult)result).Value.GetType());
        }

        private AuthenticationController GetSubject()
        {
            _mockCardManager = new Mock<ICardManager>();
            _mockMapper = new Mock<IMapper> ();
            _mockTokenManager = new Mock<ITokenManager> ();
            _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository> ();

            return new AuthenticationController(_mockCardManager.Object, _mockMapper.Object,
                _mockTokenManager.Object, _mockRefreshTokenRepository.Object);
        }
    }
}
