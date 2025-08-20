using Common.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Pharmacist.Repository.Interfaces
{
    public interface IPharmacistRepository : IDapperRepositoryBase<Pharmacist.Repository.Models.Pharmacist>
    {
        IEnumerable<Models.Pharmacist> GetPharmacists(long pharmacyid);
        Models.Pharmacist GetByEmail(string email);
    }
}
