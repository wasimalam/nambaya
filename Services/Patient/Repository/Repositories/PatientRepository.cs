using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Dapper;
using Patient.Repository.Interfaces;
using System.Data;
using System.Linq;

namespace Patient.Repository.Repositories
{
    public class PatientRepository : DapperRepositoryBase<Models.Patient>, IPatientRepository
    {
        public PatientRepository(IDatabaseSession session) : base(session)
        {

        }
        public Models.Patient GetByPharmacyPatientID(string pharmacypatientid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where PharmacyPatientID = @pharmacypatientid", new { pharmacypatientid = pharmacypatientid }).FirstOrDefault();
        }
        new public PagedResults<Models.Patient> GetAllPages(int limit, int offset, string orderBy = null, IFilter parameters = null)
        {
            var secProps = GetSecuredProperties(typeof(Models.Patient));
            var whereClause = GenerateWhereClause(parameters).Replace("StatusId ", "pc.StatusId ").Replace("IsActive ", "p.IsActive ");
            var limitClause = limit != 0 ? $"Limit @Limit Offset @Offset" : "";
            var results = new PagedResults<Models.Patient>();
            DynamicParameters par = new DynamicParameters();
            var sql = $"SELECT *  FROM {TableName} {GenerateOrderByClause(orderBy)} {limitClause}; SELECT COUNT(*)  FROM {TableName}";
            var connection = DatabaseSession.Session;
            if (limit != 0)
            {
                par.Add("Limit", limit);
                par.Add("Offset", offset);
            }
            if (connection is Microsoft.Data.SqlClient.SqlConnection)
            {
                limitClause = limit != 0 ? $" OFFSET  @Offset ROWS FETCH NEXT @Limit ROWS ONLY " : "";
                sql = $"SELECT p.*  FROM {TableName} p {whereClause} {GenerateOrderByClause(orderBy)} {limitClause}; SELECT COUNT(*) FROM {TableName} {whereClause}";
            }
            if (parameters != null)
            {
                foreach (var p in parameters.Params)
                {
                    System.Reflection.PropertyInfo securedProp = null;
                    foreach (var sec in secProps)
                    {
                        if (p.Key.ToLower().Contains(sec.Name.ToLower()))
                        {
                            securedProp = sec;
                            break;
                        }
                    }
                    if (securedProp != null)
                    {
                        var ca = securedProp.GetCustomAttributes(typeof(DapperSecured), true).FirstOrDefault() as DapperSecured;
                        par.Add(p.Key, dbType: ca.DbType, size: ca.Length, direction: ParameterDirection.Input, value: p.Value);
                    }
                    else
                        par.Add(p.Key, p.Value);
                }
            }
            var f = connection.QueryMultiple(sql, par);
            results.Data = f.Read<Models.Patient>().ToList();
            results.TotalCount = f.Read<int>().FirstOrDefault();
            results.Data.ToList().ForEach(p => p.DataState = DBState.None);
            results.PageSize = limit == 0 ? results.TotalCount : limit;
            return results;
        }
        public PagedResults<Models.Patient> GetAllPatientCasePages(int limit, int offset, string orderBy = null, IFilter parameters = null)
        {
            var secProps = GetSecuredProperties(typeof(Models.Patient));
            var whereClause = GenerateWhereClause(parameters).Replace("StatusId ", "pc.StatusId ").Replace("IsActive ", "p.IsActive ").Replace("caseIDString ", "pc.Id ");
            var orderbyClause = GenerateOrderByClause(orderBy ?? "casestartdate desc");
            var limitClause = limit != 0 ? $"Limit @Limit Offset @Offset" : "";
            var results = new PagedResults<Models.Patient>();
            DynamicParameters par = new DynamicParameters();
            var sql = $"SELECT *  FROM {TableName} {orderbyClause} {limitClause}; SELECT COUNT(*)  FROM {TableName}";
            var connection = DatabaseSession.Session;
            if (limit != 0)
            {
                par.Add("Limit", limit);
                par.Add("Offset", offset);
            }
            if (connection is Microsoft.Data.SqlClient.SqlConnection)
            {
                limitClause = limit != 0 ? $" OFFSET  @Offset ROWS FETCH NEXT @Limit ROWS ONLY " : "";
                sql = $"SELECT p.*, pc.id as caseid, pc.startdate as casestartdate, pc.enddate as caseenddate, pc.isactive as caseisactive, d.deviceid, " +
                    $" pc.Statusid, pc.Stepid, " +
                    $" pc.CardiologistID as CardiologistID, pc.DoctorID, pedf.CreatedOn as EDFUploadDate, qer.QuickResultID as QuickResultID" +
                    $" FROM {TableName} p" +
                    $" inner join PatientCases pc on p.ID = pc.patientid left join DeviceAssignment d on pc.id = d.patientcaseid and d.isAssigned=1 left join PatientEDFFile pedf on pc.id = pedf.patientcaseid " +
                    $" left join QuickEvaluationResult qer on pc.id = qer.patientcaseid " +
                    $" {whereClause} {orderbyClause} {limitClause};" +
                    $" SELECT COUNT(*) FROM {TableName} p inner join PatientCases pc on p.ID = pc.patientid left join DeviceAssignment d on pc.id = d.patientcaseid and d.isAssigned=1 left join PatientEDFFile pedf on pc.id = pedf.patientcaseid" +
                    $" left join QuickEvaluationResult qer on pc.id = qer.patientcaseid" +
                    $" {whereClause}";
            }
            if (parameters != null)
            {
                foreach (var p in parameters.Params)
                {
                    System.Reflection.PropertyInfo securedProp = null;
                    foreach (var sec in secProps)
                    {
                        if (p.Key.ToLower().Contains(sec.Name.ToLower()))
                        {
                            securedProp = sec;
                            break;
                        }
                    }
                    if (securedProp != null)
                    {
                        var ca = securedProp.GetCustomAttributes(typeof(DapperSecured), true).FirstOrDefault() as DapperSecured;
                        par.Add(p.Key, dbType: ca.DbType, size: ca.Length, direction: ParameterDirection.Input, value: p.Value);
                    }
                    else
                        par.Add(p.Key, p.Value);
                }
            }
            var f = connection.QueryMultiple(sql, par);
            results.Data = f.Read<Models.Patient>().ToList();
            results.TotalCount = f.Read<int>().FirstOrDefault();
            results.Data.ToList().ForEach(p => p.DataState = DBState.None);
            results.PageSize = limit == 0 ? results.TotalCount : limit;
            return results;
        }
        public Models.Patient GetByPatientCaseID(long patientcaseid)
        {
            var sql = $"SELECT p.*, pc.id as caseid, pc.startdate as casestartdate, pc.enddate as caseenddate, pc.isactive as caseisactive, d.deviceid, " +
                    $" pc.Statusid, pc.Stepid, pc.CardiologistID as CardiologistID , pc.DoctorID, pedf.CreatedOn as EDFUploadDate,qef.QuickResultID" +
                    $" FROM {TableName} p" +
                    $" inner join PatientCases pc on p.ID = pc.patientid left join DeviceAssignment d on pc.id = d.patientcaseid and d.isAssigned=1 left join PatientEDFFile pedf on pc.id = pedf.patientcaseid left join QuickEvaluationResult qef on pc.id = qef.patientcaseid" +
                    $" where pc.id= @patientcaseid order by d.Id desc";

            return GetItems(System.Data.CommandType.Text, sql, new { patientcaseid = patientcaseid }).FirstOrDefault();
        }
        public Models.Patient GetByAssignedDeviceId(long deviceid)
        {
            var sql = $"SELECT p.*, pc.id as caseid, pc.startdate as casestartdate, pc.enddate as caseenddate, pc.isactive as caseisactive, d.deviceid, " +
                    $" pc.Statusid, pc.Stepid, pc.CardiologistID as CardiologistID, pc.DoctorID, pedf.CreatedOn as EDFUploadDate " +
                    $" FROM {TableName} p" +
                    $" inner join PatientCases pc on p.ID = pc.patientid inner join DeviceAssignment d on pc.id = d.patientcaseid and d.isAssigned=1 left join PatientEDFFile pedf on pc.id = pedf.patientcaseid" +
                    $" where d.DeviceID = @deviceid order by d.Id desc";

            return GetItems(System.Data.CommandType.Text, sql, new { deviceid = deviceid }).FirstOrDefault();
        }
        public void Delete(long patientId)
        {
            var sql = $"delete  pa from MedicationPlanFile pa inner join patientcases pc on pa.patientcaseid= pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete  pa from PatientAdditionalInfo pa inner join patientcases pc on pa.patientcaseid= pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete pa from CaseDispatchDetail pa inner join patientcases pc on pa.patientcaseid = pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete pa from DeviceAssignment pa inner join patientcases pc on pa.patientcaseid = pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete dd from DrugFreeText dd inner join DrugGroup pa on dd.DrugGroupID = pa.ID " +
                $"inner join patientcases pc on pa.patientcaseid = pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete dd from DrugReceipe dd inner join DrugGroup pa on dd.DrugGroupID = pa.ID  " +
                $"inner join patientcases pc on pa.patientcaseid = pc.id; " +
                $"delete di from DrugIngredients di inner join DrugDetails dd on di.Drugdetailsid = dd.id " +
                $"inner join DrugGroup pa on dd.DrugGroupID = pa.ID  inner join patientcases pc on pa.patientcaseid = pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete dd from DrugDetails dd inner join DrugGroup pa on dd.DrugGroupID = pa.ID " +
                $"inner join patientcases pc on pa.patientcaseid = pc.id inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete pa from DrugGroup pa inner join patientcases pc on pa.patientcaseid = pc.id inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete pa from QuickEvaluationFile pa inner join patientcases pc on pa.patientcaseid = pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete pa from QuickEvaluationResult pa inner join patientcases pc on pa.patientcaseid = pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete pa from CaseNotes pa inner join patientcases pc on pa.patientcaseid = pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"/*delete pa from CaseAudit pa inner join patientcases pc on pa.patientcaseid = pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid;*/ " +
                $"delete pa from PatientEDFFile pa inner join patientcases pc on pa.patientcaseid = pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete pa from DetailEvaluation pa inner join patientcases pc on pa.patientcaseid = pc.id " +
                $"inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete pc from PatientCases pc inner join patient pt on pc.patientid = pt.id where pt.id = @patientid; " +
                $"delete pt from Patient pt where pt.id = @patientid; ";
            Execute(CommandType.Text, sql, new { patientid = patientId });
        }
    }
}
