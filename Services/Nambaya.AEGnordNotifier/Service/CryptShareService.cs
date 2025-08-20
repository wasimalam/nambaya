using System;
using System.Collections.Generic;
using System.IO;
using Common.BusinessObjects.ConsumerMessages;
using Cryptshare.API;
using Nambaya.AEGnordNotifier.Logger;
using Nambaya.AEGnordNotifier.Settings;

namespace Nambaya.AEGnordNotifier.Service
{
    public class CryptShareService
    {
        private readonly ConfigurationStore _configuration;
        private readonly RabbitMqService _rabbitMqService;

        public CryptShareService()
        {
            _configuration = new ConfigurationStore();
            _rabbitMqService = new RabbitMqService();
        }

        public bool LogClientId()
        {
            try
            {
                var client = new Client(
                    _configuration.CryptShareSenderEmail,
                    new CryptshareConnection(new WebServiceUri(_configuration.CryptShareServerUrl)),
                    Path.GetTempPath()
                );

                LogHelper.Log.Information($"{nameof(LogClientId)}: Please make sure following client id has added in crypt share server. Otherwise, crypt share service will fail to send files.");
                LogHelper.Log.Information($"Client Id: {client.ClientId}");

                return true;
            }
            catch (Exception e)
            {
                LogHelper.Log.Error($"{nameof(LogClientId)}: Failed to log crypt share client id. Error details: {e}");
                throw;
            }
           

            
        }

        public void SendFiles(PatientCaseDispatchMessage message, List<string> filePaths)
        {
            LogHelper.Log.Information($"{nameof(SendFiles)}: About to send file through crypt share for patient case Id:{message.PatientCaseId:D6}");
            try
            {
                var client = new Client(
                _configuration.CryptShareSenderEmail,
                new CryptshareConnection(new WebServiceUri(_configuration.CryptShareServerUrl)),
                Path.GetTempPath());

                var transfer = new Transfer
                {
                    SenderName = _configuration.CryptShareSenderName,
                    SenderPhone = _configuration.CryptShareSenderPhoneNumber,
                    Subject = $"{_configuration.CryptShareSubject}-{message.PatientCaseId:D6}" ,
                    Recipients = GetRecipients(),
                    Files = filePaths,
                    SendEmails = true,
                    NotifySender = true,
                    NotifyRecipients = true,
                    InformAboutDownload = true,
                    PasswordMode = PasswordMode.Manual,
                    Password = _configuration.CryptSharePassword,
                };

                // Ensure that the client is verified.
                CheckVerificationResult checkVerificationResult = client.CheckVerification();
                if (!checkVerificationResult.Verified)
                {
                    if (checkVerificationResult.VerificationMode == VerificationMode.Client)
                    {
                        LogHelper.Log.Error($"Please authorize the following Client ID on the Crypt share Server:{client.ClientId}");
                        throw new Exception(
                            $"Please authorize the following Client ID on the Crypt share Server:{client.ClientId}");
                    }

                    throw new Exception(
                        $"Verification has not set to client mode for crypt share.");
                }


                // Set the transfer expiration date to the maximum storage duration.
                var policy = client.RequestPolicy(transfer.Recipients.ConvertAll(recipient => recipient.EmailAddress));
                transfer.ExpirationDate = DateTime.Now.AddDays(policy.StorageDuration);
                client.BeginTransfer(transfer,
                    null,
                    null,
                    (e) => { HandleUploadInterrupted(message, filePaths); },
                    () => { HandleUploadCancelled(message, filePaths); },
                    3600,
                    () => { UploadFilesFinishedCallback(message, filePaths); }
                    );
            }
            catch (Exception e)
            {
                LogHelper.Log.Error($"Failed to send reports. Error details: {e}");
                _rabbitMqService.PublishPatientCaseDetail(message, false);
            }
            

        }

        private List<Recipient> GetRecipients()
        {
            var recipients = new List<Recipient>();

            var emailAddresses = _configuration.RecipientEmailAddresses.Split(',');

            foreach (var emailAddress in emailAddresses)
            {
                recipients.Add(new Recipient(emailAddress));
            }

            return recipients;
        }

        private void HandleUploadInterrupted(PatientCaseDispatchMessage message, List<string> filePaths)
        {
            DeleteFiles(filePaths);
            _rabbitMqService.PublishPatientCaseDetail(message, false);
        }
        private void HandleUploadCancelled(PatientCaseDispatchMessage message, List<string> filePaths)
        {
            DeleteFiles(filePaths);
            _rabbitMqService.PublishPatientCaseDetail(message, false);
        }

        private void UploadFilesFinishedCallback(PatientCaseDispatchMessage message, List<string> filePaths)
        {
            DeleteFiles(filePaths);
            _rabbitMqService.PublishPatientCaseDetail(message, true);
        }

        private void DeleteFiles(List<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                File.Delete(filePath);
            }
        }

    }
}

