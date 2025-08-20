using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using NambayaUser.Contracts.Interfaces;
using Patient.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NambayaUser.Service
{
    public class PatientService : BaseService, IPatientService
    {
        private readonly WebServiceConfiguration _webApiConf;
        public PatientService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
        }
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
