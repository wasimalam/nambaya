using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.Helpers;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Interfaces;
using Patient.Contracts.Models;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Patient.Service
{
    public class PatientImportService : BaseService, IPatientImportService
    {
        private readonly IPatientService _patientService;
        private readonly IDrugService _drugService;
        private readonly IAdditionalInfoService _additionalInfoService;
        private StringBuilder _xmlErrors = new StringBuilder();
        private readonly ILogger<PatientImportService> _logger;
        public PatientImportService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _patientService = serviceProvider.GetService<IPatientService>();
            _drugService = serviceProvider.GetService<IDrugService>();
            _additionalInfoService = serviceProvider.GetService<IAdditionalInfoService>();
            _logger = serviceProvider.GetRequiredService<ILogger<PatientImportService>>();
        }

        public PatientBO ImportXml(byte[] fileContent, string pharmacypatientid, string patientEmail, string patientPhone, string fileName, string fileContentType)
        {
            _logger.LogInformation($"Entered in ImportXML pharmacypatientid {pharmacypatientid}, patientEmail {patientEmail} fileName {fileName} fileContentType{fileContentType}" );
            bool bXML = fileContentType == "text/xml";
            var sessionContext = GetSessionContext();
            using (var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    long caseid = 0;
                    if (bXML)
                    {
                        caseid = InternalImport(sessionContext.PharmacyID, pharmacypatientid, patientEmail, patientPhone, fileName, Encoding.UTF8.GetString(fileContent), caseid);
                        _logger.LogInformation($"ImportXml: caseId {caseid}");
                    }
                    else
                    {
                        //
                        MemoryStream ms = new MemoryStream(fileContent);
                        var unzippedStreams = CompressionHelper.UnZipStreamWithName(ms);
                        byte[] pdfFile = null;
                        string pdfFilename="";
                        foreach (var unzipStream in unzippedStreams)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                unzipStream.stream.CopyTo(memoryStream);
                                if (memoryStream.Length > 0)
                                {
                                    byte[] streamData = memoryStream.ToArray();
                                    if (streamData[0] == '%' && streamData[1] == 'P' && streamData[2] == 'D' && streamData[3] == 'F' && streamData[4] == '-')
                                    {
                                        pdfFile = streamData;
                                        pdfFilename = unzipStream.fileName;
                                    }
                                    else if (ValidXML(streamData))
                                        caseid = InternalImport(sessionContext.PharmacyID, pharmacypatientid, patientEmail,patientPhone, fileName, Encoding.UTF8.GetString(memoryStream.ToArray()), caseid);
                                }
                            }
                        }
                        if (pdfFile != null)
                        {
                            _drugService.UploadMedicationFile(
                                new MedicationPlanFileBO
                                {
                                    PatientCaseID = caseid,
                                    FilePath = $"MP/{caseid}/{Guid.NewGuid()}",
                                    FileName = pdfFilename,
                                    FileLength = pdfFile.Length,
                                    ContentType = "application/pdf",
                                    FileData = pdfFile
                                }
                            );
                        }
                    }
                    if (caseid == 0)
                        throw new ServiceException(ClientSideErrors.INVALID_PATIENT_IMPORT_DATA);
                    unitOfWork.Commit();
                    return _patientService.GetPatientbyCaseID(caseid);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    if (ex is System.IO.InvalidDataException)
                        throw new ServiceException("INVALID_FILE_FORMAT");
                    throw ex.InnerException ?? ex;
                }
            }
            throw new ServiceException(ClientSideErrors.INVALID_PATIENT_IMPORT_DATA);
        }
        private long InternalImport(long pharmacyid, string pharmacypatientid, string patientEmail, string patientPhone, string fileName, string fileXmlContent, long caseid)
        {
            _logger.LogInformation($"InternalImport: pharmacyid{pharmacyid} pharmacypatientid {pharmacypatientid},patientPhone {patientPhone}, patientEmail {patientEmail} fileName {fileName} fileXmlContent {fileXmlContent}");

            bool bPatientAdded = caseid != 0;
            if (Validate(fileName, pharmacypatientid, fileXmlContent))
            {
                MedicationPlanBO drug = fileXmlContent.XmlDeserializeFromString<MedicationPlanBO>();
                if (!bPatientAdded)
                {
                    drug.Patient.PharmacyPatientID = pharmacypatientid;
                    drug.Patient.Email = patientEmail;
                    drug.Patient.Phone = patientPhone;
                    drug.Patient.IsActive = true;
                    drug.Patient.PharmacyID = pharmacyid;
                    _patientService.AddPatient(drug.Patient, bOverrideExisting: true);
                    _logger.LogInformation("Patient addeed sucessfully ");
                }
                else
                    drug.Patient.CaseID = caseid;
                if (drug.PatientAdditionalInfo != null)
                {
                    drug.PatientAdditionalInfo.PatientCaseID = drug.Patient.CaseID;
                    var existingAdditionalInfo = _additionalInfoService.GetPatientAdditionalInfoByCaseID(drug.Patient.CaseID);
                    _logger.LogInformation($"existing Additional info of patient {Newtonsoft.Json.JsonConvert.SerializeObject(existingAdditionalInfo)}");
                    if (existingAdditionalInfo == null)
                    {
                        _logger.LogInformation($"existing Additional info of patient not found add new addtitonal info ");
                        _additionalInfoService.AddPatientAdditionalInfo(drug.PatientAdditionalInfo);
                    }
                    else
                    {
                        bool bUpdate = false;
                        if (string.IsNullOrWhiteSpace(drug.PatientAdditionalInfo.AllergiesFreeText) == false)
                        {
                            existingAdditionalInfo.AllergiesFreeText = $"{(string.IsNullOrWhiteSpace(existingAdditionalInfo.AllergiesFreeText) ? (existingAdditionalInfo.AllergiesFreeText + "\n") : "")}{drug.PatientAdditionalInfo.AllergiesFreeText}";
                            bUpdate = true;
                        }
                        if (string.IsNullOrWhiteSpace(drug.PatientAdditionalInfo.AdditionalFreeText) == false)
                        {
                            existingAdditionalInfo.AdditionalFreeText = $"{(string.IsNullOrWhiteSpace(existingAdditionalInfo.AdditionalFreeText) ? (existingAdditionalInfo.AdditionalFreeText + "\n") : "")}{drug.PatientAdditionalInfo.AdditionalFreeText}";
                            bUpdate = true;
                        }
                        if (bUpdate)
                            _additionalInfoService.UpdatePatientAdditionalInfo(existingAdditionalInfo);
                    }
                }
                if (drug.DrugGroups != null)
                {
                    foreach (var drugGroup in drug.DrugGroups)
                    {
                        drugGroup.PatientCaseID = drug.Patient.CaseID;
                        _drugService.AddDrugGroup(drugGroup);
                    }
                }
                return drug.Patient.CaseID;
            }
            throw new ServiceException(ClientSideErrors.INVALID_PATIENT_IMPORT_DATA);
        }
        private bool ValidXML(byte[] content)
        {
            try
            {
                //Create the schema validating reader.
                using (XmlReader reader = XmlReader.Create(new StringReader(Encoding.UTF8.GetString(content))))
                {
                    while (reader.Read()) { }
                }

                if (_xmlErrors.Length > 0)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        private bool Validate(string filename, string patientpharmacyid, string xmlFileContext)
        {
            try
            {
                _logger.LogInformation($"Validate: filename {filename} patientpharmacyid {patientpharmacyid} xmlFileContext {xmlFileContext} ");
                TextReader textReader = File.OpenText("bmp_V2.6_ext_QT-Life_eng.xsd");
                XmlSchema xmlSchema = XmlSchema.Read(textReader, ValidationCallBack);

                _logger.LogInformation($"Validate: set the validation settings");
                // Set the validation settings.
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.DtdProcessing = DtdProcessing.Parse;
                settings.Schemas.Add(xmlSchema);
                _logger.LogInformation($"Validate: xml schemea added in settings");


                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                settings.ValidationType = ValidationType.Schema;

                //Create the schema validating reader.
                using (XmlReader reader = XmlReader.Create(new StringReader(xmlFileContext), settings))
                {
                    while (reader.Read()) { }
                }

                if (_xmlErrors.Length > 0)
                {
                    _logger.LogError($"Patient {patientpharmacyid} Import data {filename} errros : {_xmlErrors}");
                    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex.InnerException != null)
                    _logger.LogError(ex.InnerException.Message);
                throw new ServiceException(ClientSideErrors.INVALID_PATIENT_IMPORT_DATA);
            }
        }

        //Display any warnings or errors.
        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            _logger.LogInformation($"ValidationCallBack called");

            if (_xmlErrors.Length == 0)
            {
                _xmlErrors.Append(String.Format("Validation Error at {0,6} {1,15} {2,100}\n", "Line", "Postion", "Error Message"));
            }

            if (args.Severity == XmlSeverityType.Warning)
                Console.WriteLine("\tWarning: Matching schema not found. No validation occurred." + args.Message);
            else
            {
                Console.WriteLine("\tValidation error: " + args.Message);
                _xmlErrors.Append(String.Format("{0,6:N0} {1,15:N0} {2,100}\n", args.Exception.LineNumber, args.Exception.LinePosition, args.Exception.Message));
            }
        }
    }
}