using CentralGroup.Contracts.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Patient.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CentralGroup.Service
{
    public class PatientService : BaseService, IPatientService
    {
        private readonly WebServiceConfiguration _webApiConf;
        private readonly IDECompletedEventNotificationService _centralGroupNotificationService;

        public PatientService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _centralGroupNotificationService = serviceProvider.GetRequiredService<IDECompletedEventNotificationService>();
        }
        public PagedResults<PatientBO> GetPatientCases(int limit, int offset, string orderby, string filter)
        {
            var sessioncontext = GetSessionContext();
            if (sessioncontext.RoleCode != RoleCodes.CentralGroupUser)
                throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
            string order = orderby == null ? "" : $"&orderby={orderby}";
            string sFilter = filter == null ? "" : $"&filter={filter}";
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient?limit={limit}&offset={offset}{sFilter}").Result;
                    return JsonSerializer.Deserialize<PagedResults<PatientBO>>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientBO GetPatientbyID(long id)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/{id}").Result;
                    return JsonSerializer.Deserialize<PatientBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientBO GetPatientbyCaseID(long patientcaseid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/cases/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<PatientBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch
            {
                throw;
            }
        }
        public PatientCaseBO GetPatientCasebyID(long patientcaseid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/getpatientcase/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<PatientCaseBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientBO AddPatient(PatientBO patient)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient", JsonSerializer.Serialize(patient)).Result;
                    return JsonSerializer.Deserialize<PatientBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientBO UpdatePatient(PatientBO patient)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient", JsonSerializer.Serialize(patient)).Result;
                    return JsonSerializer.Deserialize<PatientBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientCaseBO UpdatePatientCase(PatientCaseBO patientCaseBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/cases", patientCaseBO).Result;
                    return JsonSerializer.Deserialize<PatientCaseBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        #region Evaluation Funtions
        public QuickEvaluationFileBO DownloadQuickEvalFile(long patientcaseid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/downloadquickevaluationfiledata?patientcaseid={patientcaseid}").Result;
                    return JsonSerializer.Deserialize<QuickEvaluationFileBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public QuickEvaluationResultBO GetQuickEvaluationResultByCaseID(long patientcaseid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/cases/quickevaluationresult/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<QuickEvaluationResultBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public DetailEvaluationBO GetDetailEvaluationByCaseID(long patientcaseid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/cases/evaluations/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<DetailEvaluationBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public byte[] DownloadDetailEvaluationFile(long patientcaseid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    return apiClient.DownloadGetAsync($"api/v1/patient/downloadecgfile?patientcaseid={patientcaseid}").Result;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        #endregion
        #region Additional Info 
        public PatientAdditionalInfoBO GetPatientAdditionalInfoByCaseID(long patientcaseid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/cases/additionalinfo/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<PatientAdditionalInfoBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public PatientAdditionalInfoBO AddPatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/cases/additionalinfo", JsonSerializer.Serialize(patientAdditionalInfo)).Result;
                    return JsonSerializer.Deserialize<PatientAdditionalInfoBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public PatientAdditionalInfoBO UpdatePatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/cases/additionalinfo", JsonSerializer.Serialize(patientAdditionalInfo)).Result;
                    return JsonSerializer.Deserialize<PatientAdditionalInfoBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        #endregion
        public CaseDispatchDetailBO AddCaseDispatchDetails(CaseDispatchDetailBO caseDispatchDetailBO)
        {
            try
            {
                var sessionContext = GetSessionContext();
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/cases/casedispatchdetails", JsonSerializer.Serialize(caseDispatchDetailBO)).Result;
                    var ret = JsonSerializer.Deserialize<CaseDispatchDetailBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    return ret;
                }
            }
            catch
            {
                throw;
            }
        }
        public CaseDispatchDetailBO GetCaseDispatchDetails(long patientcaseid)
        {
            try
            {
                var sessionContext = GetSessionContext();
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/cases/casedispatchdetails?patientcaseid={patientcaseid}").Result;
                    var ret = JsonSerializer.Deserialize<CaseDispatchDetailBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    return ret;
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch
            {
                throw;
            }
        }
        #region drug related functions
        public List<DrugGroupBO> GetDrugGroups(long patientcaseid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/druggroups/patientcase/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<List<DrugGroupBO>>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugGroupBO GetDrugGroup(long druggroupid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/druggroups/{druggroupid}").Result;
                    return JsonSerializer.Deserialize<DrugGroupBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public DrugGroupBO AddDrugGroup(DrugGroupBO drugGroupBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/druggroups", JsonSerializer.Serialize(drugGroupBO)).Result;
                    return JsonSerializer.Deserialize<DrugGroupBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public void DeleteDrugGroup(DrugGroupBO drugGroupBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.DeleteInternalAsync($"api/v1/patient/druggroups", JsonSerializer.Serialize(drugGroupBO)).Result;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugDetailsBO AddDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/druggroups/details", JsonSerializer.Serialize(drugDetailsBO)).Result;
                    return JsonSerializer.Deserialize<DrugDetailsBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugDetailsBO UpdateDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/druggroups/details", JsonSerializer.Serialize(drugDetailsBO)).Result;
                    return JsonSerializer.Deserialize<DrugDetailsBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public void DeleteDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.DeleteInternalAsync($"api/v1/patient/druggroups/details", JsonSerializer.Serialize(drugDetailsBO)).Result;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugIngredientsBO AddDrugIngredients(DrugIngredientsBO drugIngredientsBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/druggroups/ingredients", JsonSerializer.Serialize(drugIngredientsBO)).Result;
                    return JsonSerializer.Deserialize<DrugIngredientsBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugFreeTextBO AddDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/druggroups/freetext", JsonSerializer.Serialize(drugFreeTextBO)).Result;
                    return JsonSerializer.Deserialize<DrugFreeTextBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugFreeTextBO UpdateDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/druggroups/freetext", JsonSerializer.Serialize(drugFreeTextBO)).Result;
                    return JsonSerializer.Deserialize<DrugFreeTextBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public void DeleteDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.DeleteInternalAsync($"api/v1/patient/druggroups/freetext", JsonSerializer.Serialize(drugFreeTextBO)).Result;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugReceipeBO AddDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/druggroups/recipe", JsonSerializer.Serialize(drugReceipeBO)).Result;
                    return JsonSerializer.Deserialize<DrugReceipeBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugReceipeBO UpdateDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.PutAsync($"api/v1/patient/druggroups/recipe", JsonSerializer.Serialize(drugReceipeBO)).Result;
                    return JsonSerializer.Deserialize<DrugReceipeBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }

        }

        public void DeleteDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.DeleteInternalAsync($"api/v1/patient/druggroups/recipe", JsonSerializer.Serialize(drugReceipeBO)).Result;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        #endregion
        #region Dashboard
        public GoalCompletedBO GetGoalcompletedPercent()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/goalcompletedpercent").Result;
                    return JsonSerializer.Deserialize<GoalCompletedBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public List<MonthlyCasesBO> GetMonthlyCasesStarted()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/monthlycasesstarted").Result;
                    return JsonSerializer.Deserialize<List<MonthlyCasesBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public List<MonthlyCasesBO> GetMonthlyCasesCompleted()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/monthlycasescompleted").Result;
                    return JsonSerializer.Deserialize<List<MonthlyCasesBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public List<MonthlyCasesBO> GetMonthlyCasesDispatched()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/monthlycasesdispatched").Result;
                    return JsonSerializer.Deserialize<List<MonthlyCasesBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public List<CaseStatsBO> GetCardiologistNotesStats()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/cardiologistnotesstats").Result;
                    return JsonSerializer.Deserialize<List<CaseStatsBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public List<QEResultStatBO> GetQEResultStats()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/qeresultstats").Result;
                    return JsonSerializer.Deserialize<List<QEResultStatBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        #endregion
    }
}
