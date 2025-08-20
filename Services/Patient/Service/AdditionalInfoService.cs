using AutoMapper;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Interfaces;
using Patient.Contracts.Models;
using Patient.Repository.Interfaces;
using Patient.Repository.Models;
using System;

namespace Patient.Service
{
    public class AdditionalInfoService : BaseService, IAdditionalInfoService
    {
        private readonly IPatientAdditionalInfoRepository _patientAdditionalInfoRepository;
        private readonly IMapper _mapper;
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<AdditionalInfoService> _logger;
        public AdditionalInfoService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
            _patientAdditionalInfoRepository = _serviceProvider.GetRequiredService<IPatientAdditionalInfoRepository>();
            _patientRepository = _serviceProvider.GetRequiredService<IPatientRepository>();
            _logger = serviceProvider.GetRequiredService<ILogger<AdditionalInfoService>>();
        }

        #region Patient Case Additional Info CRUD
        PatientAdditionalInfoBO IAdditionalInfoService.GetPatientAdditionalInfoByCaseID(long patientcaseid)
        {
            _logger.LogInformation($"GetPatientAdditionalInfoByCaseID: patient case id {patientcaseid}");
            var additionalInfo = _patientAdditionalInfoRepository.GetByPatientCaseId(patientcaseid);
            return _mapper.Map<PatientAdditionalInfoBO>(additionalInfo);
        }
        long IAdditionalInfoService.AddPatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfoBO)
        {
            _logger.LogInformation($"AddPatientAdditionalInfo: patient case id {patientAdditionalInfoBO.PatientCaseID}");
            var sessionContext = GetSessionContext();
            var additionalInfo = _mapper.Map<PatientAdditionalInfo>(patientAdditionalInfoBO);
            additionalInfo.CreatedBy = sessionContext.LoginName;
            var pat = _mapper.Map<PatientBO>(_patientRepository.GetByPatientCaseID(patientAdditionalInfoBO.PatientCaseID));
            if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                && (pat.PharmacyID != sessionContext.PharmacyID))
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            _patientAdditionalInfoRepository.Insert(additionalInfo);
            return additionalInfo.ID;
        }
        void IAdditionalInfoService.UpdatePatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfoBO)
        {
            _logger.LogInformation($"UpdatePatientAdditionalInfo: start updating {patientAdditionalInfoBO.PatientCaseID}");
            var sessionContext = GetSessionContext();
            var pat = _mapper.Map<PatientBO>(_patientRepository.GetByPatientCaseID(patientAdditionalInfoBO.PatientCaseID));
            if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                && (pat.PharmacyID != sessionContext.PharmacyID))
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            var additionalInfo = _patientAdditionalInfoRepository.GetByID(patientAdditionalInfoBO.ID);
            additionalInfo.AdditionalFreeText = patientAdditionalInfoBO.AdditionalFreeText;
            additionalInfo.AllergiesFreeText = patientAdditionalInfoBO.AllergiesFreeText;
            additionalInfo.IsFeeding = patientAdditionalInfoBO.IsFeeding;
            additionalInfo.IsPregnant = patientAdditionalInfoBO.IsPregnant;
            additionalInfo.Weight = patientAdditionalInfoBO.Weight;
            additionalInfo.Height = patientAdditionalInfoBO.Height;
            additionalInfo.CreatinineValue = patientAdditionalInfoBO.CreatinineValue;
            additionalInfo.IsNurseCase = patientAdditionalInfoBO.IsNurseCase;
            additionalInfo.HealthStatus = patientAdditionalInfoBO.HealthStatus;
            additionalInfo.UpdatedBy = sessionContext.LoginName;
            _patientAdditionalInfoRepository.Update(additionalInfo);
            _logger.LogInformation($"UpdatePatientAdditionalInfo: Update completed");

        }
        #endregion //Patient Case Additional Info CRUD


    }
}