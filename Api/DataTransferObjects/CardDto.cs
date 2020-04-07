namespace FirstCateringAuthenticationApi.DataTransferObjects
{
    public class CardDto
    {
        public string CardNumber { get; set; }

        public string FullName { get; set; }

        public string Bearer { get; set; }
        
        public string RefreshToken { get; set; }
    }
}