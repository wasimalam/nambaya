using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patient.API.Hubs
{
    public class QuickEvalProgressHub : Hub
    {
        public static Dictionary<string, List<string>> ConnectionCaseMap { get; private set; }
        public QuickEvalProgressHub(IHubContext<QuickEvalProgressHub> hubContext)
        {
            if (ConnectionCaseMap == null)
                ConnectionCaseMap = new Dictionary<string, List<string>>();
        }
        [HubMethodName("RegisterForPatient")]
        public async Task RegisterForPatient(string patientCaseId)
        {
            await Task.Run(() =>
            {
                if (ConnectionCaseMap.ContainsKey(patientCaseId))
                    ConnectionCaseMap[patientCaseId].Add(Context.ConnectionId);
                else
                    ConnectionCaseMap.Add(patientCaseId, new List<string>() { Context.ConnectionId });
            });
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var item in ConnectionCaseMap.Where(kvp => kvp.Value.Contains(Context.ConnectionId)).ToList())
            {
                if (ConnectionCaseMap[item.Key].Count == 1)
                    ConnectionCaseMap.Remove(item.Key);
                else
                    ConnectionCaseMap[item.Key].Remove(Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
    public class RegisterData
    {
        public long PatientCaseId { get; set; }
    }
}
