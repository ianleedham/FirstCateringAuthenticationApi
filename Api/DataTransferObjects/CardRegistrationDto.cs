using System.ComponentModel.DataAnnotations;

namespace FirstCateringAuthenticationApi.DataTransferObjects
{
    public class CardRegistrationDto
    {
        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Pin { get; set; }

    }
}
