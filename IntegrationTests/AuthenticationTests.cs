using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FirstCateringAuthenticationApi.DataTransferObjects;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Xunit.Sdk;

namespace IntegrationTests
{
    public class AuthenticationTests : IClassFixture<CustomWebApplicationFactory<FirstCateringAuthenticationApi.Startup>>
    {
        private readonly CustomWebApplicationFactory<FirstCateringAuthenticationApi.Startup> _factory;

        public AuthenticationTests(CustomWebApplicationFactory<FirstCateringAuthenticationApi.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_tap_unauthorised()
        {
            // Arrange
            var client = _factory.CreateClient();
            var dto = new TapDto()
            {
                CardNumber = "1234ewqs6789oiuy"
            };
            var json = JsonConvert.SerializeObject(dto);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            
            // Act
            var response = await client.PostAsync("/authentication/tap", byteContent);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task Post_login_notFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var loginDto = new LoginParametersDto()
            {
                CardNumber = "1234ewqs6789o88y",
                Pin = "5678"
            };
            var loginJson = JsonConvert.SerializeObject(loginDto);
            var loginBuffer = System.Text.Encoding.UTF8.GetBytes(loginJson);
            var loginByteContent = new ByteArrayContent(loginBuffer);
            loginByteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            // Act
            var response = await client.PostAsync("/authentication/login", loginByteContent);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Post_register_ok()
        {
            // Arrange
            var client = _factory.CreateClient();
            var dto = new CardRegistrationDto()
            {
                CardNumber = "1234ewqs6789oiuy",
                EmployeeId = "147656",
                FirstName = "test",
                LastName = "rodeo",
                PhoneNumber = "07945832712",
                Email = "Ian.leedham@ehsdata.com",
                Pin = "5678"
            };
            var json = JsonConvert.SerializeObject(dto);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            // Act
            var response = client.PostAsync("/authentication/register", byteContent).Result;

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task Post_register_badRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var dto = new CardRegistrationDto()
            {
                CardNumber = "123434s6789odiuy",
                EmployeeId = "123456",
                FirstName = "test",
                LastName = "rodeo",
                PhoneNumber = "07945832712",
                Email = "Ian.leedham@ehsdata.com",
                Pin = "5678"
            };
            var json = JsonConvert.SerializeObject(dto);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            await client.PostAsync("/authentication/register", byteContent);
            
            // Act
            var response = await client.PostAsync("/authentication/register", byteContent);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task Post_login_ok()
        {
            // Arrange
            var client = _factory.CreateClient();
            
            // register
            var registerDto = new CardRegistrationDto()
            {
                CardNumber = "123be4qs6ou9oiuy",
                EmployeeId = "127856",
                FirstName = "test",
                LastName = "rodeo",
                PhoneNumber = "07945832712",
                Email = "Ian.leedham@ehsdata.com",
                Pin = "5678"
            };
            var registerJson = JsonConvert.SerializeObject(registerDto);
            var registerBuffer = System.Text.Encoding.UTF8.GetBytes(registerJson);
            var registerByteContent = new ByteArrayContent(registerBuffer);
            registerByteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var registerResult = 
                client.PostAsync("/authentication/register", registerByteContent).Result;
            
            // login card
            var loginDto = new LoginParametersDto()
            {
                CardNumber = registerDto.CardNumber,
                Pin = registerDto.Pin
            };
            var loginJson = JsonConvert.SerializeObject(loginDto);
            var loginBuffer = System.Text.Encoding.UTF8.GetBytes(loginJson);
            var loginByteContent = new ByteArrayContent(loginBuffer);
            loginByteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (registerResult.IsSuccessStatusCode)
            { 
                // Act
                var response = client.PostAsync("/authentication/login", loginByteContent).Result;

                // Assert
                Assert.True(response.StatusCode == HttpStatusCode.OK);
            }
            else
            {
                Assert.True(registerResult.IsSuccessStatusCode);
            }
        }
    }
}