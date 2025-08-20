using AutoMapper;
using Patient.Contracts.Models;

namespace Patient.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Repository.Models.Patient, PatientBO>().ReverseMap();
            CreateMap<Repository.Models.PatientCases, PatientCaseBO>().ReverseMap();
            CreateMap<Repository.Models.PatientEDFFile, PatientEDFFileBO>().ReverseMap();
            CreateMap<Repository.Models.QuickEvaluationFile, QuickEvaluationFileBO>().ReverseMap();
            CreateMap<Repository.Models.QuickEvaluationResult, QuickEvaluationResultBO>().ReverseMap();
            CreateMap<Repository.Models.DetailEvaluation, DetailEvaluationBO>().ReverseMap();
            CreateMap<Repository.Models.PatientAdditionalInfo, PatientAdditionalInfoBO>().ReverseMap();
            CreateMap<Repository.Models.CaseDispatchDetail, CaseDispatchDetailBO>().ReverseMap();
            CreateMap<Repository.Models.DrugGroup, DrugGroupBO>().ReverseMap();
            CreateMap<Repository.Models.DrugDetails, DrugDetailsBO>().ReverseMap();
            CreateMap<Repository.Models.DrugIngredients, DrugIngredientsBO>().ReverseMap();
            CreateMap<Repository.Models.DrugFreeText, DrugFreeTextBO>().ReverseMap();
            CreateMap<Repository.Models.DrugReceipe, DrugReceipeBO>().ReverseMap();
            CreateMap<Repository.Models.QuickEvaluationResult, CaseNotesBO>().ReverseMap();
            CreateMap<Repository.Models.DetailEvaluation, CaseNotesBO>().ReverseMap();
            CreateMap<Repository.Models.CaseNotes, CaseNotesBO>().ReverseMap();
            CreateMap<Repository.Models.DeviceAssignment, DeviceAssignmentBO>().ReverseMap();
            CreateMap<Repository.Models.MedicationPlanFile, MedicationPlanFileBO>().ReverseMap();
            CreateMap<Repository.Models.Doctor, DoctorBO>().ReverseMap();
        }
    }
}