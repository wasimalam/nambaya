using Common.DataAccess.Interfaces;
using Pharmacist.Repository.Models;

namespace Pharmacist.Repository.Interfaces
{
    public interface IPharmacyRepository : IDapperRepositoryBase<Pharmacy>
    {
        Pharmacy GetByIdentification(string identification);
        Pharmacy GetByEmail(string email);
        Pharmacy GetByPharmacist(string pharmacistemail);
        long GetCount();
    }
}
