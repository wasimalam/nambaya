using System;
using System.Configuration;

namespace Nambaya.AEGnordNotifier.Settings
{
    public class ConfigurationStore
    {
        #region Fields

        private readonly object _loadingLock = new object();
        private bool _isLoaded;
        private KeyValueConfigurationCollection _settings;

        #endregion

        #region Properties
        
        
        public string ApplicationName
        {
            get
            {
                EnsureLoaded();
                return _settings["ApplicationName"].Value;
            }
        }
        
        public string IdentityServerUri
        {
            get
            {
                EnsureLoaded();
                return _settings["IdentityServerUri"].Value;
            }
        }
        public string PatientServiceUri
        {
            get
            {
                EnsureLoaded();
                return _settings["PatientServiceUri"].Value;
            }
        }
        public string RabbitMqHost
        {
            get
            {
                EnsureLoaded();
                return _settings["RabbitMqHost"].Value;
            }
        }
        public string RabbitMqVirtualHost
        {
            get
            {
                EnsureLoaded();
                return _settings["RabbitMqVirtualHost"].Value;
            }
        }

        public string RabbitMqUsername
        {
            get
            {
                EnsureLoaded();
                return _settings["RabbitMqUsername"].Value;
            }
        }

        public string RabbitMqPassword
        {
            get
            {
                EnsureLoaded();
                return _settings["RabbitMqPassword"].Value;
            }
        }

        public string RabbitMqQueueName
        {
            get
            {
                EnsureLoaded();
                return _settings["RabbitMqQueueName"].Value;
            }
        }

        public string RabbitMqConnectionName
        {
            get
            {
                EnsureLoaded();
                return _settings["RabbitMqConnectionName"].Value;
            }
        }

        public string RabbitMqPort
        {
            get
            {
                EnsureLoaded();
                return _settings["RabbitMqPort"].Value;
            }
        }
        public string ApiName
        {
            get
            {
                EnsureLoaded();
                return _settings["ApiName"].Value;
            }
        }


        public string ClientId
        {
            get
            {
                EnsureLoaded();
                return _settings["ClientId"].Value;
            }
        }


        public string ClientSecret
        {
            get
            {
                EnsureLoaded();
                return _settings["ClientSecret"].Value;
            }
        }


        public string CryptShareSenderEmail
        {
            get
            {
                EnsureLoaded();
                return _settings["CryptShareSenderEmail"].Value;
            }
        }

        public string Scope
        {
            get
            {
                EnsureLoaded();
                return _settings["Scope"].Value;
            }
        }
        public string CryptShareSenderName
        {
            get
            {
                EnsureLoaded();
                return _settings["CryptShareSenderName"].Value;
            }
        }
        public string CryptShareSenderPhoneNumber
        {
            get
            {
                EnsureLoaded();
                return _settings["CryptShareSenderPhoneNumber"].Value;
            }
        }
        public string CryptShareServerUrl
        {
            get
            {
                EnsureLoaded();
                return _settings["CryptShareServerUrl"].Value;
            }
        }
        public string RecipientEmailAddresses
        {
            get
            {
                EnsureLoaded();
                return _settings["RecipientEmailAddresses"].Value;
            }
        }
        public string CryptShareSubject
        {
            get
            {
                EnsureLoaded();
                return _settings["CryptShareSubject"].Value;
            }
        }
        public string CryptSharePassword
        {
            get
            {
                EnsureLoaded();
                return _settings["CryptSharePassword"].Value;
            }
        }

        public bool IsSsl
        {
            get
            {
                EnsureLoaded();
                return _settings["CryptSharePassword"].Value == "true";
            }
        }
    


        #endregion

        #region Methods

        private void EnsureLoaded()
        {
            lock (_loadingLock)
            {
                if (_isLoaded)
                {
                    return;
                }

                string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                var map = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
                Configuration configurationManager = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                AppSettingsSection appSettingsSection = configurationManager.AppSettings;
                _settings = appSettingsSection.Settings;

                _isLoaded = true;
            }
        }



        #endregion
    }
}
