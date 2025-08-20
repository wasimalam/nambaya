using Common.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pharmacist.Contracts.Interfaces;
using System;
using System.Text.Json;

namespace Pharmacist.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PatientImportController : ControllerBase
    {
        private readonly ILogger<PatientImportController> _logger;
        private IPatientImportService _patientService;

        public PatientImportController(ILogger<PatientImportController> logger, IPatientImportService patientservice)
        {
            _logger = logger;
            _patientService = patientservice;
        }

        [HttpPost, DisableRequestSizeLimit]
        public ActionResult ImportPatientFile(ImportPatientRequest importPatientRequest)
        {
            try
            {
                var ret = _patientService.ImportPatient(importPatientRequest.ClientId, importPatientRequest.Secret,
                    new Contracts.Models.PharmacyBO()
                    {
                        Identification = importPatientRequest.PharmacyIdentificationNumber,
                        Name = importPatientRequest.PharmacyName,
                        Contact = importPatientRequest.PharmacyContact,
                        Email = importPatientRequest.PharmacyEmail,
                        Phone = importPatientRequest.PharmacyPhone
                    }, importPatientRequest.CaseNumber, importPatientRequest.PatientEmail, 
                    importPatientRequest.PatientPhone, Convert.FromBase64String(importPatientRequest.FileData));
                return Ok(ret);
            }
            catch (Exception ex)
            {
                importPatientRequest.FileData = "";
                _logger.LogError($"Import Patient failed, Request:{JsonSerializer.Serialize(importPatientRequest)}");
                _logger.LogError(ex, ex.Message);
                if (ex is ServiceException)
                    return BadRequest(ex.Message);
                return BadRequest("REQUEST_FAILURE");
            }
        }
    }
    public class ImportPatientRequest
    {
        public string ClientId { get; set; } // Constant string for Pharma4U - pharma4u_client
        public string Secret { get; set; } // Const string for Pharma4U - B575B263-8A48-48EE-97EB-1B79FE734900
        public string CaseNumber { get; set; }
        public string PharmacyIdentificationNumber { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyContact { get; set; }
        public string PharmacyEmail { get; set; }
        public string PharmacyPhone { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhone { get; set; }
        public string FileData { get; set; }  // It will be always zip file in base64 string
    }
}