namespace FirstCateringAuthenticationApi.DataTransferObjects
{
    /// <summary>
    /// passed back to the client for registered loggedin cards
    /// </summary>
    public class CardDto
    {
        /// <summary>
        /// The cards number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// The users full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The jwt/bearer token
        /// </summary>
        public string Bearer { get; set; }
        
        /// <summary>
        /// The refresh token for the jwt
        /// </summary>
        public string RefreshToken { get; set; }
    }
}