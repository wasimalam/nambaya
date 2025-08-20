namespace Cardiologist.Contracts.Models
{
    public class UpdateSignatureOtpRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public string Token { get; set; }
        public string ImageData { get; set; }
    }
}