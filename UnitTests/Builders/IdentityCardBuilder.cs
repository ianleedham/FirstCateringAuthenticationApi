using System;
using DAL.Models;

namespace UnitTests.Builders
{
    public class IdentityCardBuilder
    {
        private string _firstName; 
        private string _lastName;
        private string _phoneNumber;
        private string _id;
        private string _userName;
        private string _email;

        public IdentityCardBuilder GenericIdentityCard()
        {
            _firstName = "Ian";
            _lastName = "Leedham";
            _phoneNumber = "+44 7926054923";
            _id = Guid.NewGuid().ToString();
            _userName = "123543";
            _email = "ian.leedham@ehsdata.com";
            return this;
        }

        public IdentityCard Build()
        {
            return new IdentityCard()
            {
                Id = _id,
                UserName = _userName,
                FirstName = _firstName,
                LastName = _lastName,
                PhoneNumber = _phoneNumber,
                Email = _email
            };
        }
    }
}