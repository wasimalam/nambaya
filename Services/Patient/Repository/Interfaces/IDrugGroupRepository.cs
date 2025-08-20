using Common.DataAccess.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public interface IDrugGroupRepository : IDapperRepositoryBase<DrugGroup>
    {
        IEnumerable<DrugGroup> GetByPatientCaseId(long patientcasedid);
    }
}
