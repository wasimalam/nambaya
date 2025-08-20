using System.Collections.Generic;
using System.ComponentModel;

namespace Common.Infrastructure
{
    public class ConfigurationConsts
    {
        public const string ApiConfigurationKey = "ApiConfiguration";
        public const string WebApiConfigurationKey = "WebServiceConfiguration";
        public const string ApiSecretConfiguration = "ApiSecretConfiguration";
        public const string CypherKey = "CypherKey";
        public const string EmailSettings = "EmailSettings";
        public const string RabbitMQSettings = "RabbitMQSettings";
        public const string SmsSettings = "SmsSettings";
        public const string EdfFilePathConfiguration = "EdfFilePathConfiguration";
        public const string NavigatorConfiguration = "NavigatorConfiguration";
        public const string TotalCasesGoal = "TotalCasesGoal";
        public const string DBCustomEncryption = "DBCustomEncryption";
        public const string UserLoginPolicy = "UserLoginPolicy";
        public const string PhoneVerificationKey = "NAMBAYA_PHONE_VERIFICATION";
        public const string UpdateSignatureVerificationKey = "SignatureSaveVerification";
        public const string DeleteSignatureVerificationKey = "SignaturDeleteVerification";
        public const string DESignatureVerificationKey = "DESignatureVerificationKey";
        public const string DeActivatePatientVerificationKey = "DeActivatePatientVerificationKey";
        public const string UpdateEDFDateTime = "UpdateEDFDateTime";
        public const string DeviceReminderAfterSecs = "DeviceReminderAfterSecs";
        public const string PharmaAssociationEmails = "PharmaAssociationEmails";
        public const string DoctorAssociationEmails = "DoctorAssociationEmails";
    }

    public class ApplicationNames
    {
        public const string NamabayaUserApp = "User";
        public const string PharmacistApp = "Pharma";
        public const string CardiologistApp = "Cardio";
        public const string CentralGroupApp = "CentralGroupApp";
    }

    public class ClaimNames
    {
        public const string ApplicationCode = "applicationcode";
        public const string RoleCode = "rolecode";
        public const string AppUserID = "appuserid";
        public const string FirstName = "firstname";
        public const string LastName = "lastname";
        public const string Email = "email";
        public const string Name = "name";
        public const string Language = "language";
        public const string PharmacyID = "pharmacyid";
        public const string CardiologistID = "cardiologistid";
    }

    public class RoleCodes
    {
        public const string Pharmacy = "Pharmacy";
        public const string Pharmacist = "Pharmacist";
        public const string CentralGroupUser = "CentralGroupUser";
        public const string Cardiologist = "Cardiologist";
        public const string NambayaUser = "NambayaUser";
        public const string Nurse = "Nurse";
        public const string PharmacyTrainer = "PharmacyTrainer";
        public const string StakeHolder = "StakeHolder";
    }
    public class SystemUsers
    {
        public const string Navigator = "Navigator";
        public const string System = "System";
    }

    public class LanguageCode
    {
        public const string English = "EN";
        public const string German = "DE";
    }

    public class NotificationType
    {
        public const long Email = 511;
        public const long SMS = 512;
    }

    public class NotificationTemplateType
    {
        public const long Email = 601;
        public const long SMS = 602;
    }
    public class NotificationEventType
    {
        public const long LoginOTP = 611;
        public const long ForgetPassword = 612;
        public const long AutoQECompletedPharmacist = 613;
        public const long QEReusltEventCardiologists = 614;
        public const long DECompletedCenterUsers = 615;
        public const long PhoneVerificationEvent = 616;
        public const long UserRegisterEvent = 617;
        public const long DeviceReminderEvent = 618;
        public const long QEReusltEventPharmaAssociation = 619;
        public const long QEReusltEventCeneterUsers = 620;
        public const long DECompletionEventDoctorAssociation = 621;
        public const long SignatureSaveEventCardiologist = 622;
        public const long DESignatureEventCardiologist = 623;
        public const long SignatureDeleteEventCardiologist = 624;
        public const long PatientDeactivateEventPharma = 625;
        public const long DECompletionEventByNurseCardiologist = 626;
        public const long ReminderDeviceCharge = 627;
    }
    public class UserSettingCodes
    {
        public const string FACTOR_NOTIFICATION_TYPE = "2FactorNotificationType";
        public const string LANGUAGE = "Language";
    }

    public class UserSettingDataTypes
    {
        public const string LOOKUPS = "Lookups";
        public const string LANGUAGE = "Language";
    }

    public class ClientSideErrors
    {
        public const string AUTHENTICATION_RETRY = "AUTHENTICATION_RETRY";
        public const string USER_ID_DELETED = "USER_ID_DELETED";
        public const string USER_ID_INACTIVE = "USER_ID_INACTIVE";
        public const string INVALID_USER_ID = "INVALID_USER_ID";
        public const string INVALID_USER_ID_PASSWORD = "INVALID_USER_ID_PASSWORD";
        public const string INVALID_IDENTITY_CONFIGURATIONS = "INVALID_IDENTITY_CONFIGURATIONS";
        public const string INVALID_USER_APPLICATION_ROLE = "INVALID_USER_APPLICATION_ROLE";
        public const string USER_ID_ALREADY_EXISTS = "USER_ID_ALREADY_EXISTS";
        public const string INVALID_DEVICE_ID = "INVALID_DEVICE_ID";
        public const string INVALID_DEVICE_ASSIGNMENT_STATUS = "INVALID_DEVICE_ASSIGNMENT_STATUS";
        public const string INVALID_DEVICE_ASSIGNMENT_DETAILS = "INVALID_DEVICE_ASSIGNMENT_DETAILS";
        public const string DEVICE_ALREADY_ASSIGNED = "DEVICE_ALREADY_ASSIGNED";
        public const string INVALID_USER_ACTION = "INVALID_USER_ACTION";
        public const string USER_NOT_AUTHORIZED = "USER_NOT_AUTHORIZED";
        public const string INVALID_PATIENT_CASE_ID = "INVALID_PATIENT_CASE_ID";
        public const string INVALID_PATIENT_CASE_STEP = "INVALID_PATIENT_CASE_STEP";
        public const string INVALID_PATIENT_IMPORT_DATA = "INVALID_PATIENT_IMPORT_DATA";
        public const string RESOURCE_NOT_FOUND = "RESOURCE_NOT_FOUND";
        public const string INVALID_DATA_TO_UPLOAD = "INVALID_DATA_TO_UPLOAD";
        public const string DEVICE_SERIAL_ALREADY_EXIST = "DEVICE_SERIAL_ALREADY_EXIST";
        public const string USER_ID_LOCKED = "USER_ID_LOCKED";
        public const string FAILED_TO_UPLOAD = "FAILED_TO_UPLOAD";
        public const string FAILED_TO_DELETE_PATIENT_CASE_DEVICE_ASSIGNED = "FAILED_TO_DELETE_PATIENT_CASE_DEVICE_ASSIGNED";
        public const string DOCTOR_EMAIL_NOT_FOUND = "DOCTOR_EMAIL_NOT_FOUND";
        public const string PHARMACY_PATIENT_ID_ALREADY_EXISTS = "PHARMACY_PATIENT_ID_ALREADY_EXISTS";
        public const string PHARMACY_IS_NOT_ACCESSIBLE = "PHARMACY_IS_NOT_ACCESSIBLE";
        public const string OTP_VERIFICATION_FAILED = "OTP_VERIFICATION_FAILED";
        public const string DOCTOR_IS_ASSOCIATED = "DOCTOR_IS_ASSOCIATED";
        public const string DOCTOR_ID_ALREADY_EXISTS = "DOCTOR_ID_ALREADY_EXISTS";
        public const string PHONE_IS_UNVERIFIED = "PHONE_IS_UNVERIFIED";
        public const string POLICY_INVALID_PASSWORD = "POLICY_INVALID_PASSWORD";
        public const string PASSWORD_RESET_REQUIRED = "PASSWORD_RESET_REQUIRED";
        public const string SIGNATURE_NOT_FOUND = "SIGNATURE_NOT_FOUND";
        public const string OLD_PASSWORD_DOES_NOT_MATCH = "OLD_PASSWORD_DOES_NOT_MATCH";
    }

    public class QuickEvaluationProgress
    {
        public const string EDF_FILE_DOWNLOADING = "EDF_FILE_DOWNLOADING";
        public const string EDF_FILE_DOWNLOADED = "EDF_FILE_DOWNLOADED";
        public const string EDF_FILE_DOWNLOAD_FAILED = "EDF_FILE_DOWNLOAD_FAILED";
        public const string EVALUATION_STARTED = "EVALUATION_STARTED";
        public const string EVALUATION_FAILED = "EVALUATION_FAILED";
        public const string EVALUATION_TIMESTAMP = "EVALUATION_TIMESTAMP";
        public const string EVALUATION_FINISHED = "EVALUATION_FINISHED";
    }
    public class CaseStatus
    {
        public const long CaseStarted = 651;
        public const long DeviceAllocated = 652;
        public const long DeviceReturned = 653;
        public const long QuickEvalInQueue = 654;
        public const long QuickEvalCompleted = 655;
        public const long DetailEvalLocked = 656;
        public const long DetailEvalUploaded = 657;
        public const long DetailEvalCompleted = 658;
        public const long ReportDispatchFailed = 659;
        public const long ReportDispatching = 660;
        public const long ReportDispatched = 661;
    }
    public class CaseStep
    {
        public const long Created = 521;
        public const long EditPatient = 522;
        public const long Medication = 523;
        public const long AdditionalInfo = 524;
        public const long DeviceAssigned = 525;
        public const long UploadEDF = 526;
        public const long QuickEvaluation = 527;
    }
    public class CardioRemarksTypes
    {
        public const long Prescription = 391;
        public const long Other = 392;
    }
    public class SerilogRabbitMQ
    {
        public const string Routing = "logging";
        public const string Exchange = "serilog-sink-exchange";
        public const string Queue = "serilog-sink-queue";
    }
    public class LoggingConstants
    {
        public const string CorrelationId = "CorrelationId";
        public const string Application = "Application";
    }
    public class DeviceStatus
    {
        public const long Available = 451;
        public const long Assigned = 452;
        public const long Inactive = 453;
    }
    public class GenderCodes
    {
        public const long Male = 401;
        public const long Female = 402;
        public const long Unknown = 403;
        public const long Diverse = 404;
    }
    public class UserOtp
    {
        public long AppUserID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string OTP { get; set; }
        public bool SMS { get; set; }
    }
    public class VerifyPhoneOtpRequest
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public string Token { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class VerifyPhoneRequest
    {
        public string PhoneNumber { get; set; }
    }
    public class LookUpValues
    {
        public int CategoryId { get; set; }
        public long Initial { get; set; }
        public long Custom { get; set; }
    }

    public class LookUpCategoryCodes
    {
        private static Dictionary<string, LookUpValues> _LookUpCategoryCodes = new Dictionary<string, LookUpValues>()
        {
            { "CITY", new LookUpValues(){ CategoryId = 1, Initial=1, Custom=10000} }, //1
            { "GENDER", new LookUpValues(){ CategoryId = 2, Initial=401, Custom=1000} }, //2
            { "DEVICE-STATUS", new LookUpValues(){ CategoryId = 3, Initial=451 } }, //3
            { "2FACTORNOTIFICATIONTYPE", new LookUpValues(){ CategoryId = 4, Initial=511 } },//4
            { "EVALUATIONSTATUS", new LookUpValues(){ CategoryId = 5, Initial=501 } },//5
            { "DRUGGROUPCODE", new LookUpValues(){ CategoryId = 6, Initial=411 } },//6
            { "QUICKREPORT", new LookUpValues(){ CategoryId = 7, Initial=515 } },//7
            { "CASESTEP", new LookUpValues(){ CategoryId = 8, Initial=521 } },//8
            { "CARDIOREMARKSTYPE", new LookUpValues(){ CategoryId = 9, Initial=391 } },//9
            { "NOTIFICATIONTEMPLATETYPE", new LookUpValues(){ CategoryId = 10, Initial=601 } },//10
            { "NOTIFICATIONEVENTTYPE", new LookUpValues(){ CategoryId = 11, Initial=611 } },//11
            { "CASESTATUS", new LookUpValues(){ CategoryId = 12, Initial=651 } } //12
        };
        public static Dictionary<string, LookUpValues> LookUpCategories
        {
            get { return _LookUpCategoryCodes; }
        }
    }
    public class QuickEvaluationProgressMessage
    {
        public long PatientCaseId { get; set; }
        public string Step { get; set; }
        public int RemainingTime { get; set; }
        public int TotalTimeElapsed { get; set; }
    }

    public enum QuickEvaluationResultEnum
    {
        [Description("Green")]
        Green = 511,
        [Description("Yellow")]
        Yellow = 512,
        [Description("Orange")]
        Orange = 513,
        [Description("Red")]
        Red = 514,
        [Description("RedRed")]
        RedRed = 515
    }
}