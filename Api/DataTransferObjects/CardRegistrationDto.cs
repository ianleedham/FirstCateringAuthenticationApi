using System.ComponentModel.DataAnnotations;

namespace FirstCateringAuthenticationApi.DataTransferObjects
{
    /// <summary>
    /// The dto for registering new cards
    /// </summary>
    public class CardRegistrationDto
    {
        /// <summary>
        /// The cards number
        /// </summary>
        [Required]
        [MaxLength(16)]
        [MinLength(16)]
        public string CardNumber { get; set; }

        /// <summary>
        /// The employees id number
        /// </summary>
        [Required]
        public string EmployeeId { get; set; }

        /// <summary>
        /// The employees first name
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// The employees last name
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// The employees email address
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// The employees phone number
        /// </summary>
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The pin number to be associated with the card
        /// </summary>
        [Required]
        [MaxLength(4)]
        [MinLength(4)]
        public string Pin { get; set; }

    }
}
