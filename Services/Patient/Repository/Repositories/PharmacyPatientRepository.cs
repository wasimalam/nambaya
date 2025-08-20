using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Dapper;
using Patient.Repository.Interfaces;
using System.Data;
using System.Linq;

namespace Patient.Repository.Repositories
{
    public class PharmacyPatientRepository : DapperRepositoryBase<Models.PharmacyPatient>, IPharmacyPatientRepository
    {
        public PharmacyPatientRepository(IDatabaseSession session) : base(session)
        {

        }
        public Models.PharmacyPatient GetByPharmacyPatientID(string pharmacyPatientID)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where pharmacyPatientID= @pharmacyPatientID", new
            {
                pharmacyPatientID
            }).FirstOrDefault();
        }
        public new void Insert(Models.PharmacyPatient obj)
        {
            var propertyContainer = ParseProperties(obj, true);
            var sql = string.Format("INSERT INTO {0} ({1}) VALUES(@{2})",
                TableName,
                string.Join(", ", propertyContainer.AllNames),
                string.Join(", @", propertyContainer.AllNames));

            DatabaseSession.Session.Query(sql, propertyContainer.AllPairs, commandType: CommandType.Text);
            obj.DataState = DBState.None;
        }
    }
}
