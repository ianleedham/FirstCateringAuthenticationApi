namespace FirstCateringAuthenticationApi.DataTransferObjects
{
    /// <summary>
    /// A dto for providing login parameters
    /// </summary>
    public class LoginParametersDto
    {
        /// <summary>
        /// The cards number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// The pin associated with the card
        /// </summary>
        public string Pin { get; set; }
    }
}