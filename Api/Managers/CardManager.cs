using FirstCateringAuthenticationApi.Classes;
using FirstCateringAuthenticationApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace FirstCateringAuthenticationApi.Managers
{
    public class CardManager : UserManager<IdentityCard>, ICardManager
    {
        public CardManager(IUserStore<IdentityCard> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<IdentityCard> passwordHasher, IEnumerable<IUserValidator<IdentityCard>> userValidators, IEnumerable<IPasswordValidator<IdentityCard>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<IdentityCard>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}
