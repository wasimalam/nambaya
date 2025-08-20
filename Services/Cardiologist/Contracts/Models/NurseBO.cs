using Common.BusinessObjects;

namespace Cardiologist.Contracts.Models
{
	public class NurseBO : BaseUserBO
	{	
		public long CardiologistID { get; set; }
		public string CompanyID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Street { get; set; }
		public string ZipCode { get; set; }
		public string Address { get; set; }
		public string County { get; set; }
		public string Phone { get; set; }
		public bool PhoneVerified { get; set; }
		public string Email { get; set; }
		public string Name
		{
			get
			{
				return FirstName + " " + LastName;
			}
		}
	}
}
