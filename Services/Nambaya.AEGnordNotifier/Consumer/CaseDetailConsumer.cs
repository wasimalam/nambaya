using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common.BusinessObjects.ConsumerMessages;
using MassTransit;
using Nambaya.AEGnordNotifier.Logger;
using Nambaya.AEGnordNotifier.Models;
using Nambaya.AEGnordNotifier.Service;
using Nambaya.AEGnordNotifier.Settings;
using Newtonsoft.Json;
using Serilog;

namespace Nambaya.AEGnordNotifier.Consumer
{
    public class CaseDetailConsumer :
        IConsumer<PatientCaseDispatchMessage>
    {
        public async Task Consume(ConsumeContext<PatientCaseDispatchMessage> context)
        {
            var message = context.Message;
            LogHelper.Log.Information($"{nameof(CaseDetailConsumer)}: Message received for patient case Id: {JsonConvert.SerializeObject(message)}, ");
            if (context.Message.PatientCaseId > 0)
            {
                var configuration = new ConfigurationStore();
                using (var service = new WebApiClientService())
                {
                    var token = await service.GetAuthorizeToken(new ApiRequestConfiguration()
                    {
                        ApiName = configuration.ApiName,
                        ClientId = configuration.ClientId,
                        Secret = configuration.ClientSecret,
                        Scope = configuration.Scope
                    }, configuration.IdentityServerUri);

                    service.SetBearerToken(token);

                    byte[] detailEvaluationFileBytes=null;
                    byte[] medicationFileBytes=null;


                    if (message.IsDetailEvaluationFileAttached)
                    {
                        LogHelper.Log.Information($"{nameof(Consume)}: About to get detail evaluation file bytes.");
                        detailEvaluationFileBytes = await service.DownloadGetAsync($"{configuration.PatientServiceUri}/api/v1/patient/DownloadECGFile?patientCaseId={context.Message.PatientCaseId}", "", "");
                        LogHelper.Log.Information($"{nameof(Consume)}: Detail evaluation file get bytes successfully..");
                    }
                    
                    if (message.IsMedicationFileAttached)
                    {
                        LogHelper.Log.Information($"{nameof(Consume)}: About to get medication file bytes.");
                        medicationFileBytes = await service.DownloadGetAsync($"{configuration.PatientServiceUri}/api/v1/patient/DownloadMedicationFile?patientCaseId={context.Message.PatientCaseId}", "", "");
                        LogHelper.Log.Information($"{nameof(Consume)}: Medication file get bytes successfully..");
                    }
                    
                    var filePaths = GetFilePaths(context.Message.PatientCaseId, detailEvaluationFileBytes, medicationFileBytes);
                    new CryptShareService().SendFiles(context.Message, filePaths);
                }
            }
            else
            {
                Log.Error($"{nameof(CaseDetailConsumer.Consume)}: Patient case Id is empty.");
            }
           
           
        }


        private List<string> GetFilePaths(long patientId, byte[] detailEvaluationFile,byte[] medicationFile)
        {
            var result = new List<string>();
            var basePath =  Path.GetTempPath();

            if (detailEvaluationFile != null)
            {
                var detailEvaluationFilePath = $"{basePath}/DetailEvaluationReport_{patientId}.pdf";
                File.WriteAllBytes(detailEvaluationFilePath, detailEvaluationFile);
                result.Add(detailEvaluationFilePath);
            }

            if (medicationFile!=null)
            {
                var medicationFilePath = $"{basePath}/MedicationFile_{patientId}.pdf";
                File.WriteAllBytes(medicationFilePath, medicationFile);
                result.Add(medicationFilePath);
            }

            return result;
        }

    }
}
