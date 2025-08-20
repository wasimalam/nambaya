namespace Pharmacist.Contracts.Models
{
    public class VerifyDeactivatePatientOtpRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public string Token { get; set; }
        public long PatientId { get; set; }
    }
}
