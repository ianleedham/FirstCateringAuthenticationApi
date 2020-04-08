using FirstCateringAuthenticationApi.DataTransferObjects;

namespace UnitTests.Builders
{
    public class CardRegistrationDtoBuilder
    {
        private string _cardNumber;
        private string _employeeId;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _pin;

        public CardRegistrationDtoBuilder GenericRegistration()
        {
            _cardNumber = "1234hgte5678inngs";
            _employeeId = "123456";
            _firstName = "Ian";
            _lastName = "Leedham";
            _email = "ian.leedham@ehsdata.com";
            _phoneNumber = "+44 7954865812";
            _pin = "1234";
            return this;
        }

        public CardRegistrationDto Build()
        {
            return new CardRegistrationDto()
            {
                CardNumber = _cardNumber,
                EmployeeId = _employeeId,
                FirstName = _firstName,
                LastName = _lastName,
                Email = _email,
                PhoneNumber = _phoneNumber,
                Pin = _pin
            };
        }
    }
}