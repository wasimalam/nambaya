using AutoMapper;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Interfaces;
using Patient.Contracts.Models;
using Patient.Repository.Interfaces;
using Patient.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Patient.Service
{
    public class PatientService : BaseService, IPatientService
    {
        private readonly IMapper _mapper;
        private readonly IPatientRepository _patientRepository;
        private readonly IDeviceAssignmentRepository _deviceAssignmentRepository;
        private readonly IPatientCasesRepository _patientCasesRepository;
        private readonly IPharmacyPatientRepository _pharmacyPatientRepository;
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _patientRepository = serviceProvider.GetRequiredService<IPatientRepository>();
            _deviceAssignmentRepository = serviceProvider.GetRequiredService<IDeviceAssignmentRepository>();
            _patientCasesRepository = serviceProvider.GetRequiredService<IPatientCasesRepository>();
            _pharmacyPatientRepository = serviceProvider.GetRequiredService<IPharmacyPatientRepository>();
            _rabbitMQClient = serviceProvider.GetRequiredService<RabbitMQClient>();
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            _logger = serviceProvider.GetRequiredService<ILogger<PatientService>>();
        }
        PagedResults<PatientBO> IPatientService.GetPatientCases(int limit, int offset, string orderby, string param)
        {
            _logger.LogInformation($"GetPatientCases: limit {limit} offset {offset} orderby {orderby} param {param}");
            var sessionContext = GetSessionContext();
            long pharmacyid = sessionContext.PharmacyID;
            _logger.LogInformation($"GetPatientCases: pharmacyid {pharmacyid}");
            PagedResults<PatientBO> pg = new PagedResults<PatientBO>();
            IFilter filter = null;
            IFilter pharmacyFilter = pharmacyid == 0 ? null : new ANDFilter(new List<IFilter>() { new Filter(strFilterName: "PharmacyID", sqlOperator: SqlOperators.Equal, strFilterValue: pharmacyid) });
            IFilter activeFilter = new ANDFilter(new List<IFilter>() { new Filter(strFilterName: "IsActive", sqlOperator: SqlOperators.Equal, 1) });
            if (sessionContext.RoleCode == RoleCodes.Cardiologist || sessionContext.RoleCode == RoleCodes.Nurse)
            {
                _logger.LogInformation($"Set filter based on role code");
                pharmacyFilter = new ANDFilter(pharmacyFilter,
                        new ORFilter(new List<IFilter>()
                        {
                            new Filter(strFilterName: "StatusID", sqlOperator: SqlOperators.Equal, strFilterValue: CaseStatus.QuickEvalCompleted),
                            new Filter(strFilterName: "CardiologistID", sqlOperator: SqlOperators.Equal, strFilterValue: sessionContext.CardiologistID)
                        }));
            }
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                var secured = expressions.Where(p => p.Property.ToLower() == "insurancenumber" || p.Property.ToLower() == "firstname"
                || p.Property.ToLower() == "lastname" || p.Property.ToLower() == "phone" || p.Property.ToLower() == "email"
                || p.Property.ToLower() == "dateofbirth");
                foreach (var i in secured)
                    i.Operation = SqlOperators.Equal;
                activeFilter = expressions.Where(p => p.Property.ToLower() == "isactive").Any() ? null : activeFilter;
                var filterlist = expressions.Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
            }
            filter = new ANDFilter(filter, new ANDFilter(activeFilter, pharmacyFilter));
            if (string.IsNullOrWhiteSpace(orderby))
                orderby = "pc.CreatedOn desc";
            if (string.IsNullOrWhiteSpace(orderby) == false && orderby.ToLower().StartsWith("name"))
            {
                var sortType = orderby.ToLower().EndsWith("asc") ? "ASC" : "DESC";
                orderby = orderby.Replace("name", $"FirstName {sortType}, LastName");
            }
            var pdb = _patientRepository.GetAllPatientCasePages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<PatientBO>(p)).ToList();
            _logger.LogInformation($"GetPatientCases: sucess");
            return pg;
        }
        PagedResults<PatientBO> IPatientService.GetCasesToDispatch(int limit, int offset, string orderby, string param)
        {
            _logger.LogInformation($"GetCasesToDispatch: limit {limit} offset{offset} orderby {orderby} param {param}");
            if (string.IsNullOrEmpty(orderby))
                orderby = "CaseStartDate Asc";
            PagedResults<PatientBO> pg = new PagedResults<PatientBO>();
            IFilter filter = null;
            _logger.LogInformation($"Setting filter for GetCasesToDispatch");
            IFilter defaultFilter = new ANDFilter(new List<IFilter>() {
                new Filter(strFilterName: "StatusID", sqlOperator: SqlOperators.Equal, strFilterValue: CaseStatus.DetailEvalCompleted),
                new Filter(strFilterName: "IsActive", sqlOperator: SqlOperators.Equal, 1)
            });
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                var secured = expressions.Where(p => p.Property.ToLower() == "insurancenumber" || p.Property.ToLower() == "firstname"
                || p.Property.ToLower() == "lastname" || p.Property.ToLower() == "phone" || p.Property.ToLower() == "email"
                || p.Property.ToLower() == "dateofbirth");
                foreach (var i in secured)
                    i.Operation = SqlOperators.Equal;
                var filterlist = expressions.Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
            }

            if (filter == null)
                filter = defaultFilter;
            else if (defaultFilter != null)
                filter = new ANDFilter(filter, defaultFilter);
            if (orderby.ToLower().StartsWith("name"))
            {
                var sortType = orderby.ToLower().EndsWith("asc") ? "ASC" : "DESC";
                orderby = orderby.Replace("name", $"FirstName {sortType}, LastName");
            }
            var pdb = _patientRepository.GetAllPatientCasePages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<PatientBO>(p)).ToList();
            _logger.LogInformation($"GetCasesToDispatch: sucess");

            return pg;
        }
        #region Patient CRUD

        long IPatientService.AddPatient(PatientBO patientBO, bool bOverrideExisting)
        {
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                _logger.LogInformation("Starting add patient ");
                try
                {
                    var sessionContext = GetSessionContext();
                    _logger.LogInformation($"SessionContext roleCode {sessionContext.RoleCode}");
                    if (sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                        patientBO.PharmacyID = sessionContext.PharmacyID;
                    patientBO.CreatedBy = sessionContext.LoginName;
                    _logger.LogInformation($"Getting Pharmacy patient against patient id {patientBO.PharmacyPatientID} ");
                    var pharmacyPatient = _pharmacyPatientRepository.GetByPharmacyPatientID(patientBO.PharmacyPatientID);
                    if (pharmacyPatient != null)
                    {
                        var oldPatient = _patientRepository.GetByPharmacyPatientID(patientBO.PharmacyPatientID);
                        if (bOverrideExisting)
                        {
                            var drugService = _serviceProvider.GetRequiredService<IDrugService>();
                            var cases = _patientCasesRepository.GetByPatientId(oldPatient.ID);
                            foreach (var c in cases)
                            {
                                if (c.StatusID != CaseStatus.CaseStarted)
                                    throw new ServiceException(ClientSideErrors.PHARMACY_PATIENT_ID_ALREADY_EXISTS);
                                drugService.DeleteMedicationPlanFiles(c.ID);
                            }
                            _patientRepository.Delete(oldPatient.ID);
                            _logger.LogInformation($"Old patient data removed from db ");
                        }
                        else
                            throw new ServiceException(ClientSideErrors.PHARMACY_PATIENT_ID_ALREADY_EXISTS);
                    }
                    else
                    {
                        _pharmacyPatientRepository.Insert(new PharmacyPatient()
                        {
                            PharmacyPatientID = patientBO.PharmacyPatientID,
                            CreatedBy = patientBO.CreatedBy
                        });
                        _logger.LogInformation($"Pharmacy patient data added ");
                    }
                    var patient = _mapper.Map<Repository.Models.Patient>(patientBO);
                    _patientRepository.Insert(patient);
                    var patcase = new PatientCases
                    {
                        PatientID = patient.ID,
                        DoctorID = patientBO.DoctorID,
                        StartDate = patientBO.CaseStartDate ?? DateTime.UtcNow,
                        StatusID = CaseStatus.CaseStarted,
                        StepID = CaseStep.Created,
                        IsActive = true,
                        CreatedBy = patientBO.CreatedBy,
                    };
                    _patientCasesRepository.Insert(patcase);
                    patientBO.CaseID = patcase.ID;
                    _unitOfWork.Commit();
                    return patient.ID;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }

        void IPatientService.UpdatePatient(PatientBO patientBO)
        {
            _logger.LogInformation($"Start updating patient data for patient id {patientBO.ID}");
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var dbPat = _patientRepository.GetByID(patientBO.ID);
                    var sessionContext = GetSessionContext();
                    if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                        && (dbPat.PharmacyID != sessionContext.PharmacyID))
                        throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                    patientBO.UpdatedBy = sessionContext.LoginName;
                    var upPat = _mapper.Map<Repository.Models.Patient>(patientBO);
                    dbPat.Title = upPat.Title;
                    //dbPat.Prefix = upPat.Prefix;
                    dbPat.FirstName = upPat.FirstName;
                    dbPat.LastName = upPat.LastName;
                    //dbPat.Suffix = upPat.Suffix;
                    dbPat.DateOfBirth = upPat.DateOfBirth;
                    dbPat.GenderID = upPat.GenderID;
                    dbPat.InsuranceNumber = upPat.InsuranceNumber;
                    dbPat.Street = upPat.Street;
                    dbPat.ZipCode = upPat.ZipCode;
                    dbPat.Address = upPat.Address;
                    dbPat.County = upPat.County;
                    dbPat.Email = upPat.Email;
                    dbPat.Phone = upPat.Phone;
                    dbPat.UpdatedBy = upPat.UpdatedBy;
                    _patientRepository.Update(dbPat);
                    _logger.LogInformation("Patient data updated sucessfully");
                    if (patientBO.CaseID != 0)
                    {
                        var pCase = _patientCasesRepository.GetByID(patientBO.CaseID);
                        if (pCase.DoctorID != patientBO.DoctorID)
                        {
                            pCase.DoctorID = patientBO.DoctorID;
                            pCase.UpdatedBy = sessionContext.LoginName;
                            _patientCasesRepository.Update(pCase);
                        }
                    }
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public PatientBO GetPatientbyCaseID(long patientcaseID)
        {
            _logger.LogInformation($"Getting patient by case id {patientcaseID} ");
            var sessionContext = GetSessionContext();
            var pat = _mapper.Map<PatientBO>(_patientRepository.GetByPatientCaseID(patientcaseID));
            if (pat == null) return null;
            if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                && (pat.PharmacyID != sessionContext.PharmacyID))
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            if (sessionContext.RoleCode != RoleCodes.CentralGroupUser && pat.CaseIsActive == false)
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            return pat;
        }

        public PatientCaseBO GetPatientCase(long patientcaseID)
        {
            var pat = _mapper.Map<PatientCaseBO>(_patientCasesRepository.GetByID(patientcaseID));
            return pat;
        }

        PatientBO IPatientService.GetPatientbyID(long patientID)
        {
            var sessionContext = GetSessionContext();
            var pat = _mapper.Map<PatientBO>(_patientRepository.GetByID(patientID));
            if (pat == null) return null;
            if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                && (pat.PharmacyID != sessionContext.PharmacyID))
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            return pat;
        }

        void IPatientService.DeletePatient(PatientBO patientBO)
        {
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    _logger.LogInformation($"Deleting patient {patientBO} ");

                    var sessionContext = GetSessionContext();
                    if (sessionContext.RoleCode != RoleCodes.CentralGroupUser)
                        throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                    var bSendCleanUpEvent = false;
                    if (patientBO != null)
                    {
                        var drugService = _serviceProvider.GetRequiredService<IDrugService>();
                        var evalService = _serviceProvider.GetRequiredService<IEvaluationService>();
                        var cases = _patientCasesRepository.GetByPatientId(patientBO.ID);
                        _logger.LogInformation("Deleting cases of patient");
                        foreach (var c in cases)
                        {
                            patientBO.CaseID = c.ID;
                            if (c.StatusID == CaseStatus.DeviceAllocated)
                                throw new ServiceException(ClientSideErrors.FAILED_TO_DELETE_PATIENT_CASE_DEVICE_ASSIGNED /*+ $"_{c.ID}"*/);
                            if (c.StatusID >= CaseStatus.QuickEvalInQueue)
                            {
                                if (c.StatusID < CaseStatus.DetailEvalCompleted)
                                    bSendCleanUpEvent = true;
                                evalService.DeleteEDFFiles(c.ID);
                            }
                            if (c.StatusID >= CaseStatus.DetailEvalCompleted)
                                evalService.DeleteDetailEvaluationFiles(c.ID);
                            drugService.DeleteMedicationPlanFiles(c.ID);
                        }
                        _patientRepository.Delete(patientBO.ID);
                        _logger.LogInformation($"Patient data sucessfully deleted");
                    }
                    _unitOfWork.Commit();
                    if (bSendCleanUpEvent)
                        RaiseNavigatorCleanEvent(patientBO);
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public void RaiseNavigatorCleanEvent(PatientBO patientBO)
        {
            EdfFileUpdatePayLoadBO edfFileUpdatePayLoadBO = new EdfFileUpdatePayLoadBO
            {
                PatientBO = patientBO,
                SessionContext = GetSessionContext(),
                CorrelationId = GetCorrelationId(),
                IsCleanup = true
            };
            _rabbitMQClient.SendMessage(KnownChannels.NAVIGATOR_EDF_FILE_UPLOADED_EVENT_CHANNEL, edfFileUpdatePayLoadBO);
        }
        public List<PatientCaseBO> GetPatientCasesByPatient(long patientid)
        {
            return _patientCasesRepository.GetByPatientId(patientid).Select(p => _mapper.Map<PatientCaseBO>(p)).ToList();
        }
        public PatientBO DeActivatePatient(long patientId)
        {
            _logger.LogInformation($"DeActivatePatient:  {patientId} ");

            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    var patient = _patientRepository.GetByID(patientId);
                    if (patient != null)
                    {
                        if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                            && (patient.PharmacyID != sessionContext.PharmacyID))
                            throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                        var drugService = _serviceProvider.GetRequiredService<IDrugService>();
                        var evalService = _serviceProvider.GetRequiredService<IEvaluationService>();
                        var cases = _patientCasesRepository.GetByPatientId(patientId);
                        _logger.LogInformation($"Deleting patient cases");
                        foreach (var c in cases)
                        {
                            if (c.StatusID == CaseStatus.DeviceAllocated)
                                throw new ServiceException(ClientSideErrors.FAILED_TO_DELETE_PATIENT_CASE_DEVICE_ASSIGNED /*+ $"_{c.ID}"*/);
                            c.IsActive = false;
                            c.UpdatedBy = sessionContext.LoginName;
                            _patientCasesRepository.Update(c);
                        }
                        patient.IsActive = false;
                        patient.UpdatedBy = sessionContext.LoginName;
                        _patientRepository.Update(patient);
                        _unitOfWork.Commit();
                        return _mapper.Map<PatientBO>(_patientRepository.GetByID(patientId));
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public PatientCaseBO UpdatePatientCase(PatientCaseBO patientCaseBO)
        {
            var sessionContext = GetSessionContext();
            var pCase = _mapper.Map<Patient.Repository.Models.PatientCases>(patientCaseBO);
            pCase.UpdatedBy = sessionContext.LoginName;
            _patientCasesRepository.Update(pCase);
            return _mapper.Map<PatientCaseBO>(_patientCasesRepository.GetByID(pCase.ID));
        }
        public PatientBO AssignPatientCase(long patientcaseid)
        {
            _logger.LogInformation($"AssignPatientCase:  {patientcaseid} ");

            var sessionContext = GetSessionContext();
            if (sessionContext.RoleCode != RoleCodes.Cardiologist && sessionContext.RoleCode != RoleCodes.Nurse)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            var patientcase = _patientCasesRepository.GetByID(patientcaseid);
            if (patientcase == null)
                return null;
            if (patientcase.CardiologistID != null && patientcase.CardiologistID > 0)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            patientcase.StatusID = CaseStatus.DetailEvalLocked;
            patientcase.CardiologistID = sessionContext.CardiologistID;
            patientcase.UpdatedBy = sessionContext.LoginName;
            _logger.LogInformation($"Case sucessfully assigned to patient");
            _patientCasesRepository.Update(patientcase);

            return _mapper.Map<PatientBO>(_patientRepository.GetByPatientCaseID(patientcaseid));
        }
        #endregion // Patient CRUD
        #region Device Assignment functions
        void IPatientService.AssignDevice(DeviceAssignmentBO deviceAssignment)
        {
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    var curassignment = _deviceAssignmentRepository.Get(deviceAssignment.DeviceID, true);
                    var patientCase = _patientCasesRepository.GetByID(deviceAssignment.PatientCaseID);
                    if (sessionContext != null)
                        patientCase.UpdatedBy = sessionContext.LoginName;
                    if (deviceAssignment.IsAssigned == false)
                    {
                        if (curassignment == null) // device should be assigned to some  
                            throw new ServiceException(ClientSideErrors.INVALID_DEVICE_ASSIGNMENT_STATUS);
                        if (curassignment.PatientCaseID != deviceAssignment.PatientCaseID)
                            throw new ServiceException(ClientSideErrors.INVALID_DEVICE_ASSIGNMENT_DETAILS);
                        curassignment.IsAssigned = deviceAssignment.IsAssigned;
                        curassignment.UpdatedBy = sessionContext.LoginName;
                        _deviceAssignmentRepository.Update(curassignment);
                        if (patientCase.StatusID == CaseStatus.DeviceAllocated)
                        {
                            patientCase.StatusID = CaseStatus.DeviceReturned;
                            _patientCasesRepository.Update(patientCase);
                        }
                    }
                    else
                    {
                        if (curassignment != null)
                            throw new ServiceException(ClientSideErrors.DEVICE_ALREADY_ASSIGNED);
                        var devAssignment = new DeviceAssignment()
                        {
                            PatientCaseID = deviceAssignment.PatientCaseID,
                            DeviceID = deviceAssignment.DeviceID,
                            IsAssigned = true,
                            AssignmentDate = deviceAssignment.AssignmentDate,
                            CreatedBy = sessionContext.LoginName
                        };
                        _deviceAssignmentRepository.Insert(devAssignment);
                        var strDeviceReminderAfterSecs = _configuration.GetSection(ConfigurationConsts.DeviceReminderAfterSecs).Value;
                        int nDeviceReminderAfterSecs = 172800; //48 hours default
                        int.TryParse(strDeviceReminderAfterSecs, out nDeviceReminderAfterSecs);
                        _rabbitMQClient.ScheduleSendMessage(KnownChannels.DEVICE_REMINDER_EVENT_CHANNEL,
                            new DeviceReminderPayLoadBO()
                            {
                                PatientBO = GetPatientbyCaseID(deviceAssignment.PatientCaseID),
                                SessionContext = sessionContext,
                                CorrelationId = GetCorrelationId(),
                                DeviceAssignment = _mapper.Map<DeviceAssignmentBO>(devAssignment)
                            },
                            new TimeSpan(0, 0, nDeviceReminderAfterSecs));
                        patientCase.StatusID = CaseStatus.DeviceAllocated;
                        _patientCasesRepository.Update(patientCase);
                    }
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public DeviceAssignmentBO GetDeviceAssignmentByCaseId(long patientcaseid)
        {
            return _mapper.Map<DeviceAssignmentBO>(_deviceAssignmentRepository.GetByCaseId(patientcaseid));
        }
        public DeviceAssignmentBO GetDeviceAssignmentById(long id)
        {
            return _mapper.Map<DeviceAssignmentBO>(_deviceAssignmentRepository.GetByID(id));
        }
        public PatientBO GetPatientByAssignedDevice(long deviceid)
        {
            return _mapper.Map<PatientBO>(_patientRepository.GetByAssignedDeviceId(deviceid));
        }
        #endregion
    }
}