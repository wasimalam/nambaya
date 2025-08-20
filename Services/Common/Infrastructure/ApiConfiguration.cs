namespace Common.Infrastructure
{
    public class ApiConfiguration
    {
        public string IdentityServerBaseUrl { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public string OidcApiName { get; set; }

        public bool CorsAllowAnyOrigin { get; set; }

        public string[] CorsAllowOrigins { get; set; }
    }
    public class WebServiceConfiguration
    {
        public string PharmacistServiceBaseUrl { get; set; }
        public string CardiologistServiceBaseUrl { get; set; }
        public string CentralGroupServiceBaseUrl { get; set; }
        public string NambayaUserServiceBaseUrl { get; set; }
        public string IdentityServerBaseUrl { get; set; }
        public string PatientServiceBaseUrl { get; set; }
        public string UserManagementServiceBaseUrl { get; set; }
        public string FileSharingServiceBaseUrl { get; set; }
        public string LoggingServiceBaseUrl { get; set; }
        public string PharmacistUIUrl { get; set; }
        public string CardiologistUIUrl { get; set; }
        public string CentralGroupUIUrl { get; set; }
        public string NambayaUserUIUrl { get; set; }
    }
    public class ApiRequestConfiguration
    {
        public string ApiName { get; set; }
        public string Secret { get; set; }
        public string Clientid { get; set; }
        public string Scope { get; set; }
    }
    public class RabbitMQSettings
    {
        public string Host { get; set; }
        public string VirtualHost { get; set; }
        public string Quartz { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ushort Port { get; set; }
        public string ConnectionName { get; set; }
        public bool SSLActive { get; set; }
        public string SSLCertificatePath { get; set; }
    }

    public class EdfFilePathConfiguration
    {
        public string EdfFilePath { get; set; }
    }

    public class NavigatorConfiguration
    {
        public string NavigatorUserName { get; set; }
        public string NavigatorPassword { get; set; }
        public string AvailableLicense { get; set; }
        public string JrePath { get; set; }
        public string NavigatorExecutableJarPath { get; set; }
        public string DatabaseManagerExecutableJarPath { get; set; }
        public string QuickEvaluationReportPath { get; set; }
        public string ProcessName { get; set; }
    }
    public class DBCustomEncryption
    {
        public string StoreProvider { get; set; }
        public string MasterKey { get; set; }
    }
    public class UserLoginPolicy
    {
        public int MaxLoginAttempts { get; set; }
        public int RequiredLength { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireDigit { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireUppercase { get; set; }
    }
}
