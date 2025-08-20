using Common.Infrastructure;
using FileSharing.Contracts;
using FileSharing.Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
#nullable enable
namespace FileSharing.Wrapper.IDgard
{
    public class IDgard : IFileServerWrapper
    {
        private ApiClient apiClient;
        private FileServerSettings _configuration;
        private BodyRequest _bodyRequest = new BodyRequest();
        private List<BoxSummary> _boxListItems;
        private string clientToken = "";
        private string loginUrl = @"webapp/rest/login";
        private string uploadTokenUrl = @"webapp/rest/boxes/##boxid##/##nodeid##?cmd=up&param=nopt&opt=nopt";
        private string uploadUrl = @"webapp/upload/##boxid##?id=##uploadtoken##&parentnodeid=##nodeid##";
        private string listUserBoxesUrl = @"webapp/rest/boxes";
        private string createBoxUrl1 = @"webapp/rest/boxes/0?mode=NEW";
        private string createBoxUrl2 = @"webapp/rest/boxes?mode=NEW";
        private string retrieveBoxUrl = @"webapp/rest/boxes/##boxid##?mode=VIEW";
        private string loadFolderUrl = @"webapp/rest/boxes/##boxid##/##nodeid##?depth=##depth##&mode=ALL";
        private string addFolderUrl = @"webapp/rest/boxes/##boxid##/##parentid##?cmd=addFolder&param=##foldername##&opt=nopt";
        private string userinfoUrl = @"webapp/rest/info";
        private string downloadTokenUrl = @"webapp/rest/boxes/##boxid##/##nodeid##?cmd=dl&param=##documentName##&opt=nopt";
        private string downloadUrl = @"webapp/rest/box/download";
        private string deleteNodeUrl = @"webapp/rest/boxes/##boxid##/##parentid##?children=##nodeid##";
        private readonly ILogger<IDgard> _logger;
        public IDgard(IOptions<FileServerSettings> fileServerSettingsAccessor, ILogger<IDgard> logger)
        {
            _configuration = fileServerSettingsAccessor.Value;
            _logger = logger;
            apiClient = new ApiClient(_configuration.BaseUrl, TimeSpan.FromMinutes(10));
            Login();
            RetrieveListBoxes();
            //RetrieveBox(this._boxListItems.LastOrDefault().name);
            //CreateFolder(new List<string> { "Test Box", "Folder11", "Folder22", "Folder33" });
            //UploadFile("Test Box/Folder11/Folder22/Folder33/Test.txt", Encoding.ASCII.GetBytes("test string"));
            //DownloadFile("Test Box/Folder11/Folder22/Folder33/Test.txt");
            //DeleteFile("Test Box/Folder11/Folder22/Folder33/Test.txt");
            //DeleteFolder("Test Box/Folder11/Folder22/Folder33");
        }
        private void Login()
        {
            this.clientToken = Guid.NewGuid().ToString();
            LoginRequest loginRequest = new LoginRequest { payload = new LoginPayload { username = _configuration.username, password = _configuration.Password } };
            var response = apiClient.PostAsync(loginUrl, JsonSerializer.Serialize(loginRequest)).Result;
            var d = JsonSerializer.Deserialize<LoginResponse>(response);
            if (d.statusCode == 101) //valid login
            {
                _bodyRequest.serverToken = d.serverToken;
                _bodyRequest.clientToken = this.clientToken;

            }
            else
                throw new System.Exception(d.statusMsg ?? "Login failed");
        }
        private void RetrieveListBoxes()
        {
            var response = apiClient.GetAsync(listUserBoxesUrl, JsonSerializer.Serialize(_bodyRequest)).Result;
            this._boxListItems = JsonSerializer.Deserialize<List<BoxSummary>>(response);
        }
        private BoxMeta CreateBox(string boxname, DateTime? expiry = null)
        {
            var response1 = apiClient.GetAsync(createBoxUrl1, JsonSerializer.Serialize(_bodyRequest)).Result;
            BoxMeta boxMeta = JsonSerializer.Deserialize<BoxMeta>(response1);

            boxMeta.name = boxname;
            var response2 = apiClient.PutAsync(createBoxUrl2, boxMeta).Result;
            return JsonSerializer.Deserialize<BoxMeta>(response2);
        }
        private bool BoxExists(string boxName)
        {
            //if (this._boxListItems == null)
            RetrieveListBoxes();
            if (_boxListItems.Where(t => t.name == boxName).Any())
                return true;
            return false;
        }
        //function creates folders mentioned in the hierarchy and returns folderid 
        private string CreateFolder(List<string> folders)
        {
            if (folders.Count > 0)
            {
                BoxMeta? boxMeta1 = RetrieveBox(folders[0]);
                if (boxMeta1 == null)
                {
                    boxMeta1 = CreateBox(folders[0]);
                    boxMeta1 = RetrieveBox(folders[0]);
                }
                if (boxMeta1 != null)
                {
                    var v = boxMeta1.root.folders;
                    var parentid = boxMeta1.root.nodeId;
                    for (int i = 1; i < folders.Count; i++)
                    {
                        if (v != null && v.Where(t => t.name == folders[i]).FirstOrDefault() != null)
                        {
                            var v1 = v.Where(t => t.name == folders[i]).FirstOrDefault().folders;
                            parentid = v.Where(t => t.name == folders[i]).FirstOrDefault().nodeId;
                            v = v1;
                        }
                        else
                        {
                            var response1 = apiClient.PostAsync(addFolderUrl.Replace("##boxid##", boxMeta1.id).Replace("##parentid##", parentid).Replace("##foldername##", folders[i]),
                                JsonSerializer.Serialize(_bodyRequest)).Result;
                            var obj = JsonSerializer.Deserialize<GeneralResponse>(response1);
                            parentid = obj.id;
                            v?.Clear();
                        }
                    }
                    return parentid;
                }
            }
            return "";
        }
        private string FolderExists(List<string> folders)
        {
            if (folders.Count > 0)
            {
                BoxMeta? boxMeta1 = RetrieveBox(folders[0]);
                if (boxMeta1 != null)
                {
                    var v = boxMeta1.root.folders;
                    var parentid = boxMeta1.root.nodeId;
                    for (int i = 1; i < folders.Count; i++)
                    {
                        if (v.Where(t => t.name == folders[i]).FirstOrDefault() != null)
                        {
                            parentid = v.Where(t => t.name == folders[i]).FirstOrDefault().nodeId;
                            v = v.Where(t => t.name == folders[i]).FirstOrDefault().folders;
                        }
                        else
                            return "";
                    }
                    return parentid;
                }
            }
            return "";
        }

        private string GetNodeId(List<string> folders)
        {
            if (folders.Count > 0)
            {
                BoxMeta? boxMeta1 = RetrieveBox(folders[0]);
                if (boxMeta1 != null)
                {
                    var response1 = apiClient.GetAsync(loadFolderUrl.Replace("##boxid##", boxMeta1.id).Replace("##nodeid##", boxMeta1.root.nodeId).Replace("##depth##", folders.Count().ToString()),
                        JsonSerializer.Serialize(_bodyRequest)).Result;
                    var userBox = JsonSerializer.Deserialize<UserBox>(response1);

                    var flds = userBox.folders;
                    var fils = userBox.files;
                    var nodeid = boxMeta1.root.nodeId;
                    for (int i = 1; i < folders.Count; i++)
                    {
                        if (flds.Where(t => t.name == folders[i]).FirstOrDefault() != null)
                        {
                            nodeid = flds.Where(t => t.name == folders[i]).FirstOrDefault().nodeId;
                            fils = flds.Where(t => t.name == folders[i]).FirstOrDefault().files;
                            flds = flds.Where(t => t.name == folders[i]).FirstOrDefault().folders;
                        }
                        else if (fils.Where(t => t.name == folders[i]).FirstOrDefault() != null)
                        {
                            nodeid = fils.Where(t => t.name == folders[i]).FirstOrDefault().nodeId;
                        }
                        else
                            return "";
                    }
                    return nodeid;
                }
            }
            return "";
        }
        private BoxMeta? RetrieveBox(string boxName)
        {
            if (BoxExists(boxName))
            {
                string boxid = (_boxListItems.FirstOrDefault(t => t.name == boxName)).id;
                var response1 = apiClient.GetAsync(retrieveBoxUrl.Replace("##boxid##", boxid), JsonSerializer.Serialize(_bodyRequest)).Result;
                return JsonSerializer.Deserialize<BoxMeta>(response1);
            }
            return null;
        }

        private void EnsureUserLoggedIn()
        {
            try
            {
                var response = apiClient.PostAsync(userinfoUrl, JsonSerializer.Serialize(new
                {
                    serverToken = _bodyRequest.serverToken,
                    clientToken = _bodyRequest.clientToken
                })).Result;
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message == "")
                    Login();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(" 401 ") || (ex.InnerException != null && ex.InnerException.Message.Contains(" 401 ")))
                {
                    _logger.LogInformation($"Trying to Login Again!!! ");
                    Login();
                    _logger.LogInformation($"Logged in Again!!! ");
                }
                else
                {
                    _logger.LogError(ex, $"Exception in EnsureUserLoggedIn");
                    throw ex;
                }
            }
        }

        public void UploadFile(string filePath, byte[] data)
        {
            EnsureUserLoggedIn();
            List<string> folders = filePath.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            string nodeid = CreateFolder(folders.GetRange(0, folders.Count - 1));
            string boxid = _boxListItems.Where(t => t.name == folders[0]).FirstOrDefault().id;
            var response1 = apiClient.PostAsync(uploadTokenUrl.Replace("##boxid##", boxid).Replace("##nodeid##", nodeid), JsonSerializer.Serialize(_bodyRequest)).Result;
            var obj1 = JsonSerializer.Deserialize<GeneralResponse>(response1);

            var response2 = apiClient.PostFileAsync(uploadUrl.Replace("##boxid##", boxid).Replace("##uploadtoken##", obj1.data).Replace("##nodeid##", nodeid),
                                                        folders.LastOrDefault(), data).Result;
            //var obj2 = JsonSerializer.Deserialize<object>(response2);
        }

        public byte[] DownloadFile(string filePath)
        {
            EnsureUserLoggedIn();
            List<string> folders = filePath.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            string nodeid = GetNodeId(folders.GetRange(0, folders.Count));
            if (string.IsNullOrWhiteSpace(nodeid))
                return null;
            string boxid = _boxListItems.Where(t => t.name == folders[0]).FirstOrDefault().id;
            string downloadData = @"downloadKey=##downloadkey##&serverToken=##servertoken##";
            var response1 = apiClient.PostAsync(downloadTokenUrl.Replace("##boxid##", boxid).Replace("##nodeid##", nodeid), JsonSerializer.Serialize(_bodyRequest)).Result;
            var obj1 = JsonSerializer.Deserialize<GeneralResponse>(response1);
            if (obj1 != null && string.IsNullOrWhiteSpace(obj1.data) == false)
                return apiClient.DownloadAsync(downloadUrl, downloadData.Replace("##downloadkey##", obj1.data).Replace("##servertoken##", this._bodyRequest.serverToken),
                    "application/x-www-form-urlencoded", "*/*").Result;
            return null;
        }

        public bool FileExists(string filePath)
        {
            EnsureUserLoggedIn();
            List<string> folders = filePath.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            string nodeid = GetNodeId(folders.GetRange(0, folders.Count));
            if (string.IsNullOrWhiteSpace(nodeid))
                return false;
            return true;
        }

        public void DeleteFile(string filepath)
        {
            EnsureUserLoggedIn();
            List<string> folders = filepath.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            string boxid = _boxListItems.Where(t => t.name == folders[0]).FirstOrDefault().id;
            string parentId = GetNodeId(folders.GetRange(0, folders.Count - 1));
            if (string.IsNullOrWhiteSpace(parentId))
                throw new Exception("File not found");
            string nodeid = GetNodeId(folders.GetRange(0, folders.Count));
            if (string.IsNullOrWhiteSpace(nodeid))
                throw new Exception("File not found");
            var response1 = apiClient.DeleteAsync(deleteNodeUrl.Replace("##boxid##", boxid).Replace("##parentid##", parentId).Replace("##nodeid##", nodeid)).Result;
        }

        public void DeleteFolder(string folderPath)
        {
            DeleteFile(folderPath);
        }
    }
}
