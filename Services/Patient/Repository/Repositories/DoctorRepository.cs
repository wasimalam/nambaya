using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Interfaces;
using System.Linq;

namespace Patient.Repository.Repositories
{
    public class DoctorRepository : DapperRepositoryBase<Models.Doctor>, IDoctorRepository
    {
        public DoctorRepository(IDatabaseSession session) : base(session)
        {

        }
        public Models.Doctor GetByDoctorID(string doctorid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where doctorid= @doctorid", new
            {
                doctorid
            }).FirstOrDefault();
        }
    }
}
