using AutoMapper;
using Common.BusinessObjects.Interfaces;
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Patient.Service
{
    public class DrugService : BaseService, IDrugService
    {
        private readonly IDrugGroupRepository _drugGroupRepository;
        private readonly IDrugDetailsRepository _drugDetailsRepository;
        private readonly IDrugIngredientsRepository _drugIngredientsRepository;
        private readonly IDrugFreeTextRepository _drugFreeTextRepository;
        private readonly IDrugReceipeRepository _drugReceipeRepository;
        private readonly IMedicationPlanFileRepository _medicationPlanFileRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ILookupService _lookupService;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly IMapper _mapper;
        private readonly ILogger<DrugService> _logger;
        public DrugService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = _serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
            _drugGroupRepository = _serviceProvider.GetRequiredService<IDrugGroupRepository>();
            _drugDetailsRepository = _serviceProvider.GetRequiredService<IDrugDetailsRepository>();
            _drugIngredientsRepository = _serviceProvider.GetRequiredService<IDrugIngredientsRepository>();
            _drugFreeTextRepository = _serviceProvider.GetRequiredService<IDrugFreeTextRepository>();
            _drugReceipeRepository = _serviceProvider.GetRequiredService<IDrugReceipeRepository>();
            _medicationPlanFileRepository = _serviceProvider.GetRequiredService<IMedicationPlanFileRepository>();
            _patientRepository = _serviceProvider.GetRequiredService<IPatientRepository>();
            _lookupService = _serviceProvider.GetRequiredService<ILookupService>();
            _logger = serviceProvider.GetRequiredService<ILogger<DrugService>>();
        }
        #region Drugs
        public List<DrugGroupBO> GetDrugGroups(long patientcaseid)
        {
            _logger.LogInformation($"GetDrugGroups: patient case id {patientcaseid}");
            var sessionContext = GetSessionContext();
            var patient = _patientRepository.GetByPatientCaseID(patientcaseid);
            if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                && (patient.PharmacyID != sessionContext.PharmacyID))
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            var drugGroups = _drugGroupRepository.GetByPatientCaseId(patientcaseid).Select(p => _mapper.Map<DrugGroupBO>(p)).ToList();
            foreach (var grp in drugGroups)
            {
                if (string.IsNullOrWhiteSpace(grp.DrugGroupName) && grp.DrugGroupCodeID > 0)
                    grp.DrugGroupName = _lookupService.GetItems("DRUGGROUPCODE").FirstOrDefault(p => p.ID == grp.DrugGroupCodeID)?.Value;
                grp.DrugDetailsList = _drugDetailsRepository.GetByDrugGroupId(grp.ID).Select(p => _mapper.Map<DrugDetailsBO>(p)).ToList();
                foreach (var det in grp.DrugDetailsList)
                    det.DrugIngredientsList = _drugIngredientsRepository.GetByDrugDetailsID(det.ID).Select(p => _mapper.Map<DrugIngredientsBO>(p)).ToList();

                grp.DrugFreeTextList = _drugFreeTextRepository.GetByDrugGroupId(grp.ID).Select(p => _mapper.Map<DrugFreeTextBO>(p)).ToList();
                grp.DrugReceipeList = _drugReceipeRepository.GetByDrugGroupId(grp.ID).Select(p => _mapper.Map<DrugReceipeBO>(p)).ToList();
            }
            return drugGroups;
        }
        public DrugGroupBO GetDrugGroupExist(long druggroupid)
        {
            _logger.LogInformation($"GetDrugGroupExist: drug ground id {druggroupid}");

            var drugGroup = _mapper.Map<DrugGroupBO>(_drugGroupRepository.GetByID(druggroupid));
            return drugGroup;
        }
        public DrugGroupBO GetDrugGroup(long druggroupid)
        {
            _logger.LogInformation($"GetDrugGroup: drug ground id {druggroupid}");
            var drugGroup = _mapper.Map<DrugGroupBO>(_drugGroupRepository.GetByID(druggroupid));
            if (string.IsNullOrWhiteSpace(drugGroup.DrugGroupName) && drugGroup.DrugGroupCodeID > 0)
                drugGroup.DrugGroupName = _lookupService.GetItems("DRUGGROUPCODE").FirstOrDefault(p => p.ID == drugGroup.DrugGroupCodeID)?.Value;
            if (drugGroup != null)
            {
                drugGroup.DrugDetailsList = _drugDetailsRepository.GetByDrugGroupId(drugGroup.ID).Select(p => _mapper.Map<DrugDetailsBO>(p)).ToList();
                foreach (var det in drugGroup.DrugDetailsList)
                    det.DrugIngredientsList = _drugIngredientsRepository.GetByDrugDetailsID(det.ID).Select(p => _mapper.Map<DrugIngredientsBO>(p)).ToList();

                drugGroup.DrugFreeTextList = _drugFreeTextRepository.GetByDrugGroupId(drugGroup.ID).Select(p => _mapper.Map<DrugFreeTextBO>(p)).ToList();
                drugGroup.DrugReceipeList = _drugReceipeRepository.GetByDrugGroupId(drugGroup.ID).Select(p => _mapper.Map<DrugReceipeBO>(p)).ToList();
            }
            else
            {
                _logger.LogInformation($"drug group not found against the group id {druggroupid}");
            }
            return drugGroup;
        }
        public long AddDrugGroup(DrugGroupBO drugGroupBO)
        {
            using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                _logger.LogInformation($"AddDrugGroup:Adding new drug group");

                try
                {
                    var sessionContext = GetSessionContext();
                    var patient = _patientRepository.GetByPatientCaseID(drugGroupBO.PatientCaseID);
                    if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                        && (patient.PharmacyID != sessionContext.PharmacyID))
                        throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                    var drugGroup = _mapper.Map<DrugGroup>(drugGroupBO);
                    drugGroup.CreatedBy = sessionContext.LoginName;
                    _drugGroupRepository.Insert(drugGroup);
                    if (drugGroupBO.DrugDetailsList != null)
                    {
                        foreach (var drugdetail in drugGroupBO.DrugDetailsList)
                        {
                            drugdetail.DrugGroupID = drugGroup.ID;
                            drugdetail.CreatedBy = sessionContext.LoginName;
                            AddDrugDetailInternal(drugdetail);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"AddDrugGroup: DrugDetailsList not found");
                    }
                    if (drugGroupBO.DrugFreeTextList != null)
                    {
                        foreach (var drugFreeText in drugGroupBO.DrugFreeTextList)
                        {
                            drugFreeText.DrugGroupID = drugGroup.ID;
                            drugFreeText.CreatedBy = sessionContext.LoginName;
                            AddDrugFreeText(drugFreeText);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"AddDrugGroup: DrugFreeTextList not found");

                    }
                    if (drugGroupBO.DrugReceipeList != null)
                    {
                        foreach (var drugReceipe in drugGroupBO.DrugReceipeList)
                        {
                            drugReceipe.DrugGroupID = drugGroup.ID;
                            drugReceipe.CreatedBy = sessionContext.LoginName;
                            AddDrugReceipe(drugReceipe);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"AddDrugGroup: DrugReceipeList not found");

                    }
                    unitOfWork.Commit();
                    return drugGroup.ID;
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public void UpdateDrugGroup(DrugGroupBO drugGroupBO)
        {

            using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                _logger.LogInformation($"UpdateDrugGroup: update existing drug group");

                try
                {
                    var sessionContext = GetSessionContext();
                    var patient = _patientRepository.GetByPatientCaseID(drugGroupBO.PatientCaseID);
                    if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                        && (patient.PharmacyID != sessionContext.PharmacyID))
                        throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                    var drugGroup = _drugGroupRepository.GetByID(drugGroupBO.ID);
                    drugGroup.DrugGroupCodeID = drugGroupBO.DrugGroupCodeID;
                    drugGroup.DrugGroupName = drugGroupBO.DrugGroupName;
                    drugGroup.UpdatedBy = sessionContext.LoginName;
                    _drugGroupRepository.Update(drugGroup);
                    _logger.LogInformation($"UpdateDrugGroup: updating compelted");

                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public void DeleteDrugGroup(DrugGroupBO drugGroupBO)
        {
            _logger.LogInformation($"DeleteDrugGroup: Delete existing drug group");

            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    var patient = _patientRepository.GetByPatientCaseID(drugGroupBO.PatientCaseID);
                    if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                        && (patient.PharmacyID != sessionContext.PharmacyID))
                        throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                    var drugGroup = _mapper.Map<DrugGroup>(drugGroupBO);
                    _drugIngredientsRepository.DeleteByGroupId(drugGroup.ID);
                    _drugDetailsRepository.DeleteByGroupId(drugGroup.ID);
                    _drugFreeTextRepository.DeleteByGroupId(drugGroup.ID);
                    _drugReceipeRepository.DeleteByGroupId(drugGroup.ID);
                    _drugGroupRepository.Delete(drugGroup);
                    _unitOfWork.Commit();
                    _logger.LogInformation($"DeleteDrugGroup: Delete existing drug group completed");

                    return;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public DrugDetailsBO GetDrugDetails(long drugdetailsid)
        {
            _logger.LogInformation($"Getting drug details against an id {drugdetailsid}");
            var drugDetails = _mapper.Map<DrugDetailsBO>(_drugDetailsRepository.GetByID(drugdetailsid));
            if (drugDetails != null)
            {
                drugDetails.DrugIngredientsList = _drugIngredientsRepository.GetByDrugDetailsID(drugDetails.ID).Select(p => _mapper.Map<DrugIngredientsBO>(p)).ToList();
            }
            return drugDetails;
        }
        public long AddDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            _logger.LogInformation($"Adding drug details  {Newtonsoft.Json.JsonConvert.SerializeObject(drugDetailsBO)}");

            using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var id = AddDrugDetailInternal(drugDetailsBO);
                    unitOfWork.Commit();
                    _logger.LogInformation($"AddDrugDetails completed");

                    return id;
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        private long AddDrugDetailInternal(DrugDetailsBO drugDetailsBO)
        {
            _logger.LogInformation($"AddDrugDetailInternal: started ");
            var sessionContext = GetSessionContext();
            var drugDetails = _mapper.Map<DrugDetails>(drugDetailsBO);
            drugDetails.CreatedBy = sessionContext.LoginName;
            _drugDetailsRepository.Insert(drugDetails);
            if (drugDetailsBO.DrugIngredientsList != null)
            {
                foreach (var drugIngredient in drugDetailsBO.DrugIngredientsList)
                {
                    drugIngredient.DrugDetailsID = drugDetails.ID;
                    drugIngredient.CreatedBy = sessionContext.LoginName;
                    AddDrugIngredients(drugIngredient);
                }
            }
            else
            {
                _logger.LogInformation($"AddDrugDetailInternal: DrugIngredientsList is empty");

            }
            return drugDetails.ID;
        }
        public void UpdateDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            _logger.LogInformation($"Updating drug details");
            using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    UpdateDrugDetailInternal(drugDetailsBO);
                    unitOfWork.Commit();
                    _logger.LogInformation($"Updating drug details completed");
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        private void UpdateDrugDetailInternal(DrugDetailsBO drugDetailsBO)
        {
            _logger.LogInformation($"UpdateDrugDetailInternal: drugDetails {Newtonsoft.Json.JsonConvert.SerializeObject(drugDetailsBO)}");
            var sessionContext = GetSessionContext();
            var drugDetailsDb = GetDrugDetails(drugDetailsBO.ID);
            var drugDetails = _mapper.Map<DrugDetails>(drugDetailsDb);
            drugDetails.PZN = drugDetailsBO.PZN;
            drugDetails.MedicineName = drugDetailsBO.MedicineName;
            drugDetails.DosageFormCode = drugDetailsBO.DosageFormCode;
            drugDetails.DosageForm = drugDetailsBO.DosageForm;
            drugDetails.DosageMorning = drugDetailsBO.DosageMorning;
            drugDetails.DosageNoon = drugDetailsBO.DosageNoon;
            drugDetails.DosageEvening = drugDetailsBO.DosageEvening;
            drugDetails.DosageNight = drugDetailsBO.DosageNight;
            drugDetails.DosageopenScheme = drugDetailsBO.DosageopenScheme;
            drugDetails.DosageUnitCode = drugDetailsBO.DosageUnitCode;
            drugDetails.DosageUnitText = drugDetailsBO.DosageUnitText;
            drugDetails.Hints = drugDetailsBO.Hints;
            drugDetails.TreatmentReason = drugDetailsBO.TreatmentReason;
            drugDetails.AdditionalText = drugDetailsBO.AdditionalText;
            drugDetails.IsActive = drugDetailsBO.IsActive;
            drugDetails.UpdatedBy = sessionContext.LoginName;
            _drugDetailsRepository.Update(drugDetails);
            foreach (var ingredientBO in drugDetailsBO.DrugIngredientsList)
            {
                var ingredientfordb = _mapper.Map<DrugIngredients>(ingredientBO);
                if (ingredientBO.ID == 0)
                {
                    ingredientfordb.DrugDetailsID = drugDetails.ID;
                    ingredientfordb.CreatedBy = sessionContext.LoginName;
                    _drugIngredientsRepository.Insert(ingredientfordb);
                    ingredientBO.ID = ingredientfordb.ID;
                    ingredientBO.DrugDetailsID = ingredientfordb.DrugDetailsID;
                    ingredientBO.CreatedBy = ingredientfordb.CreatedBy;
                    ingredientBO.CreatedOn = ingredientfordb.CreatedOn;
                    drugDetailsDb.DrugIngredientsList.Add(_mapper.Map<DrugIngredientsBO>(ingredientfordb));
                }
                else
                {
                    var ingredientfromdb = drugDetailsDb.DrugIngredientsList.FirstOrDefault(p => p.ID == ingredientfordb.ID);
                    if (ingredientfromdb.ActiveIngredients != ingredientfordb.ActiveIngredients ||
                        ingredientfromdb.Strength != ingredientfordb.Strength)
                    {
                        ingredientfordb.UpdatedBy = sessionContext.LoginName;
                        _drugIngredientsRepository.Update(ingredientfordb);
                        ingredientBO.UpdatedBy = ingredientfordb.UpdatedBy;
                        ingredientBO.UpdatedOn = ingredientfordb.UpdatedOn;
                        drugDetailsDb.DrugIngredientsList.Remove(ingredientfromdb);
                        drugDetailsDb.DrugIngredientsList.Add(_mapper.Map<DrugIngredientsBO>(ingredientfordb));
                    }
                }
            }
            foreach (var ingredientBO in drugDetailsDb.DrugIngredientsList)
            {
                var ingredientfromdb = _mapper.Map<DrugIngredients>(ingredientBO);
                var ingredientfordb = drugDetailsBO.DrugIngredientsList.FirstOrDefault(p => p.ID == ingredientfromdb.ID);
                if (ingredientfordb == null)
                {
                    ingredientfromdb.UpdatedBy = sessionContext.LoginName;
                    _drugIngredientsRepository.Delete(ingredientfromdb);
                }
            }
        }
        public void DeleteDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            _logger.LogInformation($"DeleteDrugDetails against id {drugDetailsBO.ID}");
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    _drugIngredientsRepository.DeleteByDrugDetailsID(drugDetailsBO.ID);
                    _drugDetailsRepository.Delete(_mapper.Map<DrugDetails>(drugDetailsBO));
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public DrugIngredientsBO GetDrugIngredients(long drugingredientsid)
        {
            return _mapper.Map<DrugIngredientsBO>(_drugIngredientsRepository.GetByID(drugingredientsid));
        }
        public long AddDrugIngredients(DrugIngredientsBO drugIngredientsBO)
        {
            var sessionContext = GetSessionContext();
            var drugIngredients = _mapper.Map<DrugIngredients>(drugIngredientsBO);
            drugIngredients.CreatedBy = sessionContext.LoginName;
            _drugIngredientsRepository.Insert(drugIngredients);
            return drugIngredients.ID;
        }
        public DrugFreeTextBO GetDrugFreeText(long drugfreetextid)
        {
            return _mapper.Map<DrugFreeTextBO>(_drugFreeTextRepository.GetByID(drugfreetextid));
        }
        public long AddDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            var sessionContext = GetSessionContext();
            var drugFreeText = _mapper.Map<DrugFreeText>(drugFreeTextBO);
            drugFreeText.CreatedBy = sessionContext.LoginName;
            _drugFreeTextRepository.Insert(drugFreeText);
            return drugFreeText.ID;
        }
        public void UpdateDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            _logger.LogInformation("Update drug free text started");
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    var drugFreetextDb = _drugFreeTextRepository.GetByID(drugFreeTextBO.ID);
                    drugFreetextDb.FreeTextInput = drugFreeTextBO.FreeTextInput;
                    drugFreetextDb.UpdatedBy = sessionContext.LoginName;
                    _drugFreeTextRepository.Update(drugFreetextDb);
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public void DeleteDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            _drugFreeTextRepository.Delete(_mapper.Map<DrugFreeText>(drugFreeTextBO));
        }
        public DrugReceipeBO GetDrugReceipe(long drugreceipeid)
        {
            return _mapper.Map<DrugReceipeBO>(_drugReceipeRepository.GetByID(drugreceipeid));
        }
        public long AddDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            _logger.LogInformation("Adding drug recipe");
            var sessionContext = GetSessionContext();
            var drugReceipe = _mapper.Map<DrugReceipe>(drugReceipeBO);
            drugReceipe.CreatedBy = sessionContext.LoginName;
            _drugReceipeRepository.Insert(drugReceipe);
            return drugReceipe.ID;
        }
        public void UpdateDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            _logger.LogInformation($"UpdateDrugReceipe: Drug Recipe {Newtonsoft.Json.JsonConvert.SerializeObject(drugReceipeBO)}");
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    var drugReceipeDb = _drugReceipeRepository.GetByID(drugReceipeBO.ID);
                    drugReceipeDb.FormulationText = drugReceipeBO.FormulationText;
                    drugReceipeDb.FormulationAdditionalText = drugReceipeBO.FormulationAdditionalText;
                    drugReceipeDb.UpdatedBy = sessionContext.LoginName;
                    _drugReceipeRepository.Update(drugReceipeDb);
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public void DeleteDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            _drugReceipeRepository.Delete(_mapper.Map<DrugReceipe>(drugReceipeBO));
        }
        public MedicationPlanFileBO GetMedicationFile(long patientcaseid)
        {
            _logger.LogInformation($"GetMedicationFile: patient case id {patientcaseid}");
            var mpFile = _medicationPlanFileRepository.GetByPatientCaseId(patientcaseid).FirstOrDefault();
            return _mapper.Map<MedicationPlanFileBO>(mpFile);
        }
        public MedicationPlanFileBO DownloadMedicationFile(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"DownloadMedicationFile: patient case id {patientcaseid}");
                var mpFile = _medicationPlanFileRepository.GetByPatientCaseId(patientcaseid).SingleOrDefault();
                MedicationPlanFileBO ret = null;
                if (mpFile != null)
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {
                        var data = apiClient.DownloadGetAsync($"api/v1/filesharing/{mpFile.FilePath}", "application/x-www-form-urlencoded", "*/*").Result;
                        ret = _mapper.Map<MedicationPlanFileBO>(mpFile);
                        ret.FileData = data.Take((int)ret.FileLength).ToArray();
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public void UploadMedicationFile(MedicationPlanFileBO patientMPFile)
        {
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                _logger.LogInformation("Uploading Medication file started");
                try
                {
                    var sessionContext = GetSessionContext();
                    if (sessionContext.RoleCode == RoleCodes.Cardiologist || sessionContext.RoleCode == RoleCodes.NambayaUser || sessionContext.RoleCode == RoleCodes.Nurse)
                        throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                    var patient = _patientRepository.GetByPatientCaseID(patientMPFile.PatientCaseID);
                    if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                        && (patient.PharmacyID != sessionContext.PharmacyID))
                        throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                    DeleteMedicationPlanFiles(patientMPFile.PatientCaseID);
                    patientMPFile.CreatedBy = sessionContext.LoginName;
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {
                        var res = apiClient.PostFileAsync($"api/v1/filesharing/{patientMPFile.FilePath}", Path.GetFileName(patientMPFile.FilePath), patientMPFile.FileData).Result;
                    }
                    _medicationPlanFileRepository.Insert(_mapper.Map<MedicationPlanFile>(patientMPFile));
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public void DeleteMedicationPlanFiles(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"DeleteMedicationPlanFiles: for patient case id {patientcaseid}");
                var mpFiles = _medicationPlanFileRepository.GetByPatientCaseId(patientcaseid);
                foreach (var f in mpFiles)
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {
                        var res = apiClient.DeleteAsync($"api/v1/filesharing/{f.FilePath}").Result;
                        _medicationPlanFileRepository.Delete(f);
                        _logger.LogInformation($"DeleteMedicationPlanFiles: Completed");

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        #endregion
    }
}