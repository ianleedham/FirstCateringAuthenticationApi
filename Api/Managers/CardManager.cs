using FirstCateringAuthenticationApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using DAL.Models;

namespace FirstCateringAuthenticationApi.Managers
{
    /// <summary>
    /// Card manager which ectends user manager
    /// </summary>
    public class CardManager : UserManager<IdentityCard>, ICardManager
    {
        /// <summary>
        /// The constructor with injected dependancies
        /// </summary>
        /// <param name="store"></param>
        /// <param name="optionsAccessor"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="userValidators"></param>
        /// <param name="passwordValidators"></param>
        /// <param name="keyNormalizer"></param>
        /// <param name="errors"></param>
        /// <param name="services"></param>
        /// <param name="logger"></param>
        public CardManager(IUserStore<IdentityCard> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<IdentityCard> passwordHasher, IEnumerable<IUserValidator<IdentityCard>> userValidators, IEnumerable<IPasswordValidator<IdentityCard>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<IdentityCard>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}
