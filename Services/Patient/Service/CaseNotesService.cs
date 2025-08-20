using AutoMapper;
using Common.Infrastructure;
using Common.Services;
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

    public class CaseNotesService : BaseService, ICaseNotesService
    {
        private readonly IQuickEvaluationResultRepository _quickEvaluationResultRepository;
        private readonly IDetailEvaluationRepository _detailEvaluationRepository;
        private readonly ICaseNotesRepository _caseNotesRepository;
        private readonly ILogger<CaseNotesService> _logger;

        private readonly IMapper _mapper;
        public CaseNotesService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
            _caseNotesRepository = _serviceProvider.GetRequiredService<ICaseNotesRepository>();
            _quickEvaluationResultRepository = _serviceProvider.GetRequiredService<IQuickEvaluationResultRepository>();
            _detailEvaluationRepository = _serviceProvider.GetRequiredService<IDetailEvaluationRepository>();
            _logger = serviceProvider.GetRequiredService<ILogger<CaseNotesService>>();
        }
        public IEnumerable<CaseNotesBO> GetCaseNotes(long patientcaseid)
        {
            _logger.LogInformation($"GetCaseNotes: Patient case id {patientcaseid}");
            List<CaseNotesBO> caseNotes = new List<CaseNotesBO>();
            var quickEvalNotes = _mapper.Map<CaseNotesBO>(_quickEvaluationResultRepository.GetByPatientCaseId(patientcaseid));
            if (quickEvalNotes != null)
            {
                quickEvalNotes.RoleCode = RoleCodes.Pharmacist;
                caseNotes.Add(quickEvalNotes);
                _logger.LogInformation($"Quick Eval notes added for role code {quickEvalNotes.RoleCode}");
            }
            var detailedEvalNotes = _mapper.Map<CaseNotesBO>(_detailEvaluationRepository.GetByPatientCaseId(patientcaseid).FirstOrDefault());
            if (detailedEvalNotes != null)
            {
                detailedEvalNotes.RoleCode = RoleCodes.Cardiologist;
                caseNotes.Add(detailedEvalNotes);
                _logger.LogInformation($"Details Eval notes added for role code {detailedEvalNotes.RoleCode}");

            }
            var centerUserCaseNotes = _caseNotesRepository.GetByPatientCaseId(patientcaseid).Select(p => _mapper.Map<CaseNotesBO>(p));
            foreach (var c in centerUserCaseNotes)
                c.RoleCode = RoleCodes.CentralGroupUser;
            caseNotes.AddRange(centerUserCaseNotes);
            return caseNotes;
        }
        public CaseNotesBO GetCaseNote(long casenoteid)
        {
            _logger.LogInformation($"Getting case notes for {casenoteid}");
            var centerUserCaseNote = _mapper.Map<CaseNotesBO>(_caseNotesRepository.GetByID(casenoteid));
            centerUserCaseNote.RoleCode = RoleCodes.CentralGroupUser;
            return centerUserCaseNote;
        }
        public long AddCaseNotes(CaseNotesBO caseNotesBO)
        {
            _logger.LogInformation($"Adding case note {Newtonsoft.Json.JsonConvert.SerializeObject(caseNotesBO)}");
            var sessionContext = GetSessionContext();
            caseNotesBO.CreatedBy = sessionContext.LoginName;
            var caseNote = _mapper.Map<CaseNotes>(caseNotesBO);
            _caseNotesRepository.Insert(caseNote);
            _logger.LogInformation("Sucessfully Added case notes");
            return caseNote.ID;
        }
    }
}
