using System;
using System.ComponentModel.DataAnnotations;

namespace FirstCateringAuthenticationApi.Model
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        
        public string CardNumber { get; set; }


        public bool Revoked { get; set; }
        
        public DateTime Expires { get; set; }
    }
}