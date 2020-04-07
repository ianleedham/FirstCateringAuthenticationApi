using FirstCateringAuthenticationApi.Controllers;
using FirstCateringAuthenticationApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Models;
using FirstCateringAuthenticationApi.DataTransferObjects;
using UnitTests.Builders;
using Xunit;

namespace UnitTests
{
    public class AuthenticationControllerTests
    {
        private Mock<ICardManager> _mockCardManager;
        private Mock<IMapper> _mapper;
        
        private readonly IdentityCardBuilder _identityCardBuilder;
        private readonly CardRegistrationDtoBuilder _cardRegistrationDtoBuilder;

        public AuthenticationControllerTests()
        {
            _identityCardBuilder = new IdentityCardBuilder();
            _cardRegistrationDtoBuilder = new CardRegistrationDtoBuilder();
        }

        [Fact]
        public async Task UserTaps_WithLoggedinUser_LogsUserOutReturnsOk()
        {
            // Arrange
            var subject = GetSubject();
            IdentityCard card = _identityCardBuilder.GenericIdentityCard().Build();
            card.Id = "cutbgln213454hbv";
            _mockCardManager.Setup(x => x.FindByIdAsync(card.Id)).ReturnsAsync(card);

            // Act

            var result = await subject.Tap(card.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(OkObjectResult), result.GetType());
            Assert.Equal(typeof(IdentityCard), (result as BadRequestObjectResult).Value.GetType());
        }
        
        [Fact]
        public async Task UserTaps_WithUnregisteredCard_ReturnsBadRequest()
        {
            // Arrange
            var subject = GetSubject();
            var cardNumber = "cutbgln213454hbv";
            _mockCardManager.Setup(x => x.FindByIdAsync(cardNumber)).ReturnsAsync((IdentityCard) null);

            // Act
            var result = await subject.Tap(cardNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(BadRequestResult), result.GetType());
            Assert.Equal("Please register your card", (result as BadRequestObjectResult).Value);
        }

        [Fact]
        public async Task UserTaps_WithRegisteredCardAndCorrectPin_LogsUserInAndReturnsCardDto()
        {
            // Arrange
            var subject = GetSubject();
            var pin = "1234";
            var card = _identityCardBuilder.GenericIdentityCard().Build();
            card.Id = "cutbgln213454hbv";
            _mockCardManager.Setup(x => x.FindByIdAsync(card.Id)).ReturnsAsync(card);
            _mockCardManager.Setup(x => x.CheckPasswordAsync(card, pin)).ReturnsAsync(true);

            // Act
            var result = await subject.TapWithPin(card.Id, pin);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(OkObjectResult), result.GetType());
            var okObject = result as OkObjectResult;
            Assert.Equal(typeof(CardDto), (okObject).Value.GetType());
            Assert.Equal("Ian Leedham", (okObject.Value as CardDto).FullName);
            Assert.Equal(card.Id, (okObject.Value as CardDto).CardNumber);
        }
        
        [Fact]
        public async Task UserTaps_WithRegisteredCardAndIncorrectPin_ReturnsUnauthorisedWithMessage()
        {
            // Arrange
            var subject = GetSubject();
            var pin = "1234";
            var card = _identityCardBuilder.GenericIdentityCard().Build();
            card.Id = "cutbgln213454hbv";
            _mockCardManager.Setup(x => x.FindByIdAsync(card.Id)).ReturnsAsync(card);
            _mockCardManager.Setup(x => x.CheckPasswordAsync(card, pin)).ReturnsAsync(false);

            // Act
            var result = await subject.TapWithPin(card.Id, pin);

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
            identityCard.Id = "cutbgln213454hbv";
            CardRegistrationDto cardRegistrationDto = _cardRegistrationDtoBuilder.GenericRegistration().Build();
            _mapper.Setup(x => x.Map<IdentityCard>(cardRegistrationDto)).Returns(identityCard);

            var identityResult = new IdentityResult();

            _mockCardManager.Setup(x => x.
                CreateAsync(identityCard, cardRegistrationDto.Pin)).ReturnsAsync(identityResult);
            _mockCardManager.Setup(x => x.FindByIdAsync(identityCard.Id)).ReturnsAsync(identityCard);

            // Act
            var result = await subject.RegisterAsync(cardRegistrationDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(typeof(OkObjectResult), result.GetType());
            Assert.Equal("Registered successfully", (result as OkObjectResult).Value);
        }

        private AuthenticationController GetSubject()
        {
            _mockCardManager = new Mock<ICardManager>();
            _mapper = new Mock<IMapper> ();

            return new AuthenticationController(_mockCardManager.Object, _mapper.Object);
        }
    }
}
