using Common.BusinessObjects;

namespace Pharmacist.Contracts.Models
{
    public class DeviceBO : BaseBO
    {
        public long? PharmacyID
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string SerialNumber
        {
            get; set;
        }
        public string Manufacturer
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        public long StatusID
        {
            get; set;
        }

        public long PatientCaseID { get; set; }
        public string PatientName { get; set; }
    }
}
