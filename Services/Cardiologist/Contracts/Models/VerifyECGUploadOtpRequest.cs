namespace Cardiologist.Contracts.Models
{
    public class VerifyECGUploadOtpRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public string Token { get; set; }
        public long PatientCaseId { get; set; }
        public string Notes { get; set; }
        public long NotesTypeId { get; set; }
    }
    public class ECGUploadRequest
    {
        public long PatientCaseId { get; set; }
        public string Notes { get; set; }
        public long NotesTypeId { get; set; }
    }
}