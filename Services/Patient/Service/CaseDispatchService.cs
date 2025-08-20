using AutoMapper;
using Common.BusinessObjects.ConsumerMessages;
using Common.DataAccess.Interfaces;
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
    public class CaseDispatchService : BaseService, ICaseDispatchService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientCasesRepository _patientCasesRepository;
        private readonly ICaseDispatchDetailRepository _caseDispatchDetailRepository;
        private readonly ILogger<CaseDispatchService> _logger;

        private readonly IMapper _mapper;
        public CaseDispatchService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
            _caseDispatchDetailRepository = _serviceProvider.GetRequiredService<ICaseDispatchDetailRepository>();
            _patientRepository = _serviceProvider.GetRequiredService<IPatientRepository>();
            _patientCasesRepository = _serviceProvider.GetRequiredService<IPatientCasesRepository>();
            _logger = serviceProvider.GetRequiredService<ILogger<CaseDispatchService>>();
        }

        #region Case Dispatch Details
        public CaseDispatchDetailBO GetCaseDispatchDetails(long patientcaseid)
        {
            _logger.LogInformation($"GetCaseDispatchDetails: patient case id {patientcaseid}");
            var caseDispatchDetail = _caseDispatchDetailRepository.GetByPatientCaseId(patientcaseid);
            return _mapper.Map<CaseDispatchDetailBO>(caseDispatchDetail);
        }
        public long AddCaseDispatchDetails(CaseDispatchDetailBO caseDispatchDetailBO)
        {
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                _logger.LogInformation($"AddCaseDispatchDetails: {Newtonsoft.Json.JsonConvert.SerializeObject(caseDispatchDetailBO)}");
                try
                {
                    var sessionContext = GetSessionContext();
                    var caseDispatchDetail = _mapper.Map<CaseDispatchDetail>(caseDispatchDetailBO);
                    Repository.Models.Patient patient = _patientRepository.GetByPatientCaseID(caseDispatchDetailBO.PatientCaseID);
                    if (patient == null)
                        throw new ServiceException(ClientSideErrors.INVALID_PATIENT_CASE_ID);
                    if (patient.StatusID < CaseStatus.DetailEvalCompleted)
                        throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
                    var pCase = _patientCasesRepository.GetByID(caseDispatchDetail.PatientCaseID);
                    //if (pCase.DoctorID == null)
                    //    throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
                    caseDispatchDetail.CreatedBy = sessionContext.LoginName;
                    pCase.StatusID = CaseStatus.ReportDispatching;
                    pCase.UpdatedBy = sessionContext.LoginName;
                    _caseDispatchDetailRepository.DeleteForCaseId(caseDispatchDetail.PatientCaseID);
                    _caseDispatchDetailRepository.Insert(caseDispatchDetail);
                    _patientCasesRepository.Update(pCase);
                    _logger.LogInformation("AddCaseDispatchDetails: added sucessfully");
                    _unitOfWork.Commit();


                    var rabbitMq = _serviceProvider.GetRequiredService<RabbitMQClient>();
                    rabbitMq.SendMessage(KnownChannels.PATIENT_AEGNORD_CHANNEL, new PatientCaseDispatchMessage()
                    {
                        PatientCaseId = caseDispatchDetailBO.PatientCaseID,
                        IsDetailEvaluationFileAttached = caseDispatchDetailBO.IsDetailEvaluationAttached,
                        IsMedicationFileAttached = caseDispatchDetailBO.IsMedicationPlanAttached,
                        CreatedBy = caseDispatchDetail.CreatedBy
                    });

                    return caseDispatchDetail.ID;

                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }

        public void UpdateCaseDetail(PatientCaseStatusDetailMessage message)
        {
            using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    _logger.LogInformation($"UpdateCaseDetail: message {Newtonsoft.Json.JsonConvert.SerializeObject(message)}");
                    Repository.Models.Patient patient = _patientRepository.GetByPatientCaseID(message.PatientCaseId);
                    if (patient == null)
                        throw new ServiceException(ClientSideErrors.INVALID_PATIENT_CASE_ID);

                    var pCase = _patientCasesRepository.GetByID(message.PatientCaseId);

                    if (message.IsSuccess)
                    {
                        _caseDispatchDetailRepository.DeleteForCaseId(message.PatientCaseId);
                        _caseDispatchDetailRepository.Insert(new CaseDispatchDetail()
                        {
                            CreatedBy = message.CreatedBy, 
                            DispatchDate = DateTime.Now,
                            PatientCaseID = message.PatientCaseId,
                            CreatedOn = DateTime.Now,
                            IsDetailEvaluationAttached = message.IsDetailEvaluationFileAttached,
                            IsMedicationPlanAttached = message.IsMedicationFileAttached
                        });
                    }

                    pCase.StatusID = message.IsSuccess 
                        ? CaseStatus.ReportDispatched
                        : CaseStatus.ReportDispatchFailed;

                    _patientCasesRepository.Update(pCase);
                    _logger.LogInformation($"Case Detail updated sucessfully ");
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }

        #endregion //Case Dispatch Details
    }
}