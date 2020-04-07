using FirstCateringAuthenticationApi.DataTransferObjects;

namespace UnitTests.Builders
{
    public class CardRegistrationDtoBuilder
    {
        public CardRegistrationDtoBuilder GenericRegistration()
        {
            
            return this;
        }

        public CardRegistrationDto Build()
        {
            return new CardRegistrationDto();
        }
    }
}