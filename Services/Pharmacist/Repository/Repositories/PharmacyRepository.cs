using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Pharmacist.Repository.Interfaces;
using Pharmacist.Repository.Models;
using System.Linq;

namespace Pharmacist.Repository.Repositories
{
    public class PharmacyRepository : DapperRepositoryBase<Pharmacy>, IPharmacyRepository
    {
        public PharmacyRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {
        }

        public Pharmacy GetByEmail(string email)
        {
            string sql = $"Select * from {TableName} where email=@email";
            return GetItems(System.Data.CommandType.Text, sql, new { email = email }).FirstOrDefault();
        }
        public Pharmacy GetByIdentification(string identification)
        {
            string sql = $"Select * from {TableName} where Identification=@identification";
            return GetItems(System.Data.CommandType.Text, sql, new { identification = identification }).FirstOrDefault();
        }
        public Pharmacy GetByPharmacist(string pharmacistemail)
        {
            string sql = $"select p.* from Pharmacy p inner join Pharmacist  c on p.ID = c.PharmacyID where c.Email=@email";
            return GetItems(System.Data.CommandType.Text, sql, new { email = pharmacistemail }).FirstOrDefault();
        }
        public long GetCount()
        {
            return ExecuteScalar<long>(System.Data.CommandType.Text, $"Select count(id) from {TableName}");
        }

    }
}
