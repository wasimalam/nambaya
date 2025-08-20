using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Pharmacist.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Pharmacist.Repository.Repositories
{
    public class PharmacistRepository : DapperRepositoryBase<Pharmacist.Repository.Models.Pharmacist>, IPharmacistRepository
    {
        public PharmacistRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {
        }
        public IEnumerable<Models.Pharmacist> GetPharmacists(long pharmacyid)
        {
            string sql = $"Select * from Pharmacist where pharmacyid={pharmacyid}";
            return GetItems(System.Data.CommandType.Text, sql, pharmacyid);
        }
        public Models.Pharmacist GetByEmail(string email)
        {
            string sql = $"Select * from Pharmacist where email=@email";
            return GetItems(System.Data.CommandType.Text, sql, new { email = email }).FirstOrDefault();
        }
    }
}
