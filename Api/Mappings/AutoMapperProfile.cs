using AutoMapper;
using DAL.Models;
using FirstCateringAuthenticationApi.DataTransferObjects;

namespace FirstCateringAuthenticationApi.Mappings
{
    /// <summary>
    /// The main mapping profile
    /// </summary>
    public class AutoMapperProfile: Profile
    {
        /// <summary>
        /// Constructor containing the mappings
        /// </summary>
        public AutoMapperProfile()
        {
            CreateMap<CardRegistrationDto, IdentityCard>()
                .ForMember(dest => dest.FirstName, o => o.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, o => o.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Id, o => o.MapFrom(src => src.CardNumber))
                .ForMember(dest => dest.UserName, o => o.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Email, o => o.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, o => o.MapFrom(src => src.PhoneNumber));

            CreateMap<IdentityCard, CardDto>()
                .ForMember(dest => dest.RefreshToken, o => o.Ignore())
                .ForMember(dest => dest.Bearer, o => o.Ignore())
                .ForMember(dest => dest.CardNumber, o => o.MapFrom(dest => dest.Id));
        }
    }
}