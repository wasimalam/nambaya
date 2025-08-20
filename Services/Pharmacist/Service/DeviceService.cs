using AutoMapper;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pharmacist.Contracts.Interfaces;
using Pharmacist.Contracts.Models;
using Pharmacist.Repository.Interfaces;
using Pharmacist.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Pharmacist.Service
{
    public class DeviceService : BaseService, IDeviceService
    {
        private readonly IPharmacistRepository _pharmacistRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly ILogger<DeviceService> _logger;
        public DeviceService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _deviceRepository = serviceProvider.GetRequiredService<IDeviceRepository>();
            _pharmacistRepository = serviceProvider.GetRequiredService<IPharmacistRepository>();
            _logger = serviceProvider.GetRequiredService<ILogger<DeviceService>>();
        }
        PagedResults<DeviceBO> IDeviceService.GetDevices(int limit, int offset, string orderby, string param)
        {
            _logger.LogInformation($"GetDevices: qLimit {limit} offset {offset} qOrderby {orderby} param {param}");
            PagedResults<DeviceBO> pg = new PagedResults<DeviceBO>();
            IFilter filter = null;
            if (!string.IsNullOrWhiteSpace(param))
            {
                _logger.LogInformation("Created filter for get devices");
                var expressions = Expression.StringToExpressions(param);
                var filterlist = expressions.Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
            }
            else
            {
                _logger.LogInformation("GetDevices: Params are empty");
            }
            var pdb = _deviceRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<DeviceBO>(p)).ToList();
            return pg;
        }
        PagedResults<DeviceBO> IDeviceService.GetDevicesByPharmacy(int limit, int offset, string orderby, string param)
        {
            _logger.LogInformation($"GetDevicesByPharmacy: qLimit {limit} offset {offset} qOrderby {orderby} param {param}");

            var sessioncontext = GetSessionContext();
            if (sessioncontext.RoleCode != RoleCodes.Pharmacist && sessioncontext.RoleCode != RoleCodes.Pharmacy)
                throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
            var pharmacyid = sessioncontext.RoleCode == RoleCodes.Pharmacist ? _pharmacistRepository.GetByID(sessioncontext.AppUserID).PharmacyID : sessioncontext.AppUserID;

            _logger.LogInformation($"GetDevicesByPharmacy: pharmacy id {pharmacyid}");
            if (string.IsNullOrWhiteSpace(orderby))
                orderby = "CreatedOn desc";

            PagedResults<DeviceBO> pg = new PagedResults<DeviceBO>();
            IFilter filter = null;
            IFilter pharmacyFilter = new ANDFilter(new List<IFilter>() { new Filter(strFilterName: "pharmacyid", sqlOperator: SqlOperators.Equal, strFilterValue: pharmacyid) });
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                var filterlist = expressions.Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
            }
            if (filter == null)
                filter = pharmacyFilter;
            else
                filter = new ANDFilter(filter, pharmacyFilter);
            var pdb = _deviceRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<DeviceBO>(p)).ToList();
            foreach (var dev in pg.Data)
            {
                if (dev.StatusID == DeviceStatus.Assigned)
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                    {
                        try
                        {
                            var res = apiClient.InternalServiceGetAsync($"api/v1/patient/getpatientbyassigneddevice/{dev.ID}").Result;
                            var pat = JsonSerializer.Deserialize<Patient.Contracts.Models.PatientBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                            dev.PatientCaseID = pat.CaseID;
                            dev.PatientName = pat.Name;
                        }
                        catch { }
                    }
                }
            }
            _logger.LogInformation("GetDevicesByPharmacy: completed");
            return pg;
        }
        long IDeviceService.AddDevice(DeviceBO device)
        {
            var p = _mapper.Map<Device>(device);
            var sessionContext = GetSessionContext();
            if (sessionContext.PharmacyID == 0)
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            p.PharmacyID = sessionContext.PharmacyID;
            p.CreatedBy = sessionContext.LoginName;
            _deviceRepository.Insert(p);
            return p.ID;
        }
        void IDeviceService.UpdateDevice(DeviceBO device)
        {
            _logger.LogInformation($"UpdateDevice: device {Newtonsoft.Json.JsonConvert.SerializeObject(device)}");
            var p = _mapper.Map<Device>(device);
            var sessionContext = GetSessionContext();
            p.UpdatedBy = sessionContext.LoginName;
            _deviceRepository.Update(p);
            _logger.LogInformation("Sucessfully update the devices");
        }
        void IDeviceService.DeleteDevice(DeviceBO device)
        {
            _logger.LogInformation($"Delete device {Newtonsoft.Json.JsonConvert.SerializeObject(device)}");
            var p = _mapper.Map<Device>(device);
            var sessionContext = GetSessionContext();
            p.UpdatedBy = sessionContext.LoginName;
            _deviceRepository.Delete(p);
        }
        DeviceBO IDeviceService.GetDeviceById(long id)
        {
            return _mapper.Map<DeviceBO>(_deviceRepository.GetByID(id));
        }

        public DeviceBO AssignDevice(DeviceAssignmentBO deviceAssignment)
        {
            _logger.LogInformation("AssignDevice: started ");
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    var sessionContext = GetSessionContext();
                    var device = _deviceRepository.GetByID(deviceAssignment.DeviceID);
                    if (device == null)
                        throw new ServiceException(ClientSideErrors.INVALID_DEVICE_ID);
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                    {
                        var res = apiClient.InternalServicePostAsync("api/v1/patient/AssignDevice", JsonSerializer.Serialize(new
                        {
                            DeviceID = deviceAssignment.DeviceID,
                            PatientCaseID = deviceAssignment.PatientCaseID,
                            AssignmentDate = deviceAssignment.AssignmentDate,
                            IsAssigned = deviceAssignment.IsAssigned
                        })).Result;
                    }
                    device.StatusID = deviceAssignment.DeviceStatusID;
                    device.UpdatedBy = sessionContext.LoginName;
                    _deviceRepository.Update(device);
                    _unitOfWork.Commit();
                    return _mapper.Map<DeviceBO>(device);
                }
                catch (AggregateException ex)
                {
                    throw ex.InnerException;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }

        }
        public DeviceAssignmentBO GetDeviceAssignmentByPatientCaseID(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"GetDeviceAssignmentByPatientCaseID: patient case id {patientcaseid}");
                var sessionContext = GetSessionContext();
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/getdeviceassignmentbycase/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<DeviceAssignmentBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public DeviceAssignmentBO GetDeviceAssignmentByID(long devAssignmentId)
        {
            try
            {
                _logger.LogInformation($"GetDeviceAssignmentByID: device assignment id  {devAssignmentId}");

                var sessionContext = GetSessionContext();
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/getdeviceassignmentbyid/{devAssignmentId}").Result;
                    return JsonSerializer.Deserialize<DeviceAssignmentBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public DeviceBO GetDeviceBySerial(string serialnumber)
        {
            _logger.LogInformation($"GetDeviceBySerial: device serial number  {serialnumber}");

            var sessioncontext = GetSessionContext();
            if (sessioncontext.RoleCode != RoleCodes.Pharmacist && sessioncontext.RoleCode != RoleCodes.Pharmacy)
                throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
            var pharmacyid = sessioncontext.RoleCode == RoleCodes.Pharmacist ? _pharmacistRepository.GetByID(sessioncontext.AppUserID).PharmacyID : sessioncontext.AppUserID;

            return _mapper.Map<DeviceBO>(_deviceRepository.GetDeviceBySerial(serialnumber, pharmacyid));
        }
    }
}
