using AutoMapper;
using Common.BusinessObjects;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Patient.Contracts.Interfaces;
using Patient.Contracts.Models;
using Patient.Repository.Interfaces;
using System;
using System.Linq;

namespace Patient.Service
{
    public class DoctorService : BaseService, IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorService> _logger;
        public DoctorService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _doctorRepository = serviceProvider.GetRequiredService<IDoctorRepository>();
            _logger = serviceProvider.GetRequiredService<ILogger<DoctorService>>();
        }
        PagedResults<DoctorBO> IDoctorService.GetDoctors(int limit, int offset, string orderby, string param)
        {
            _logger.LogInformation($"Get Doctors: limit {limit} offset {offset} orderby {orderby} param {param}");
            PagedResults <DoctorBO> pg = new PagedResults<DoctorBO>();
            IFilter filter = null;
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                if (expressions.Where(p => p.Property.ToLower() == "name").Any())
                    expressions.Where(p => p.Property.ToLower() == "name").FirstOrDefault().Property = "FirstName +' '+LastName";
                var filterlist = expressions.Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
            }
            if (string.IsNullOrWhiteSpace(orderby))
                orderby = "CreatedOn desc";
            if (string.IsNullOrWhiteSpace(orderby) == false && orderby.ToLower().StartsWith("name"))
                orderby = orderby.Replace("name", "FirstName+' '+LastName");
            var pdb = _doctorRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<DoctorBO>(p)).ToList();
            _logger.LogInformation($"Get Doctors: Completed");
            return pg;
        }
        DoctorBO IDoctorService.GetDoctorById(long id)
        {
            var doc = _mapper.Map<DoctorBO>(_doctorRepository.GetByID(id));
            return doc;
        }

        long IDoctorService.AddDoctor(DoctorBO doctor)
        {
            _logger.LogInformation($"AddDoctor: doctor {Newtonsoft.Json.JsonConvert.SerializeObject(doctor)}");
            var p = _mapper.Map<Repository.Models.Doctor>(doctor);
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    SessionContext sessionContext = GetSessionContext();
                    if (!string.IsNullOrWhiteSpace(doctor.DoctorID) && _doctorRepository.GetByDoctorID(doctor.DoctorID) != null)
                        throw new ServiceException(ClientSideErrors.DOCTOR_ID_ALREADY_EXISTS);
                    p.CreatedBy = sessionContext.LoginName;
                    _doctorRepository.Insert(p);
                    _unitOfWork.Commit();
                    return p.ID;
                }
                catch //(Exception e)
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
        }

        void IDoctorService.UpdateDoctor(DoctorBO doctor)
        {
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                _logger.LogInformation($"Updating doctor started for doctor id {doctor.ID}");
                try
                {
                    SessionContext sessionContext = GetSessionContext();
                    var dbDoctor = _doctorRepository.GetByID(doctor.ID);
                    if (!string.IsNullOrWhiteSpace(dbDoctor.DoctorID) && !string.IsNullOrWhiteSpace(doctor.DoctorID)
                        && dbDoctor.DoctorID.Trim().ToLower() != doctor.DoctorID.Trim().ToLower())
                        throw new ServiceException(ClientSideErrors.DOCTOR_ID_ALREADY_EXISTS);

                    if (string.IsNullOrWhiteSpace(dbDoctor.DoctorID) && !string.IsNullOrWhiteSpace(doctor.DoctorID) && _doctorRepository.GetByDoctorID(doctor.DoctorID) != null)
                        throw new ServiceException(ClientSideErrors.DOCTOR_ID_ALREADY_EXISTS);
                    dbDoctor.FirstName = doctor.LastName;
                    dbDoctor.LastName = doctor.LastName;
                    dbDoctor.DoctorID = doctor.DoctorID;
                    dbDoctor.CompanyID = doctor.CompanyID;
                    dbDoctor.Street = doctor.Street;
                    dbDoctor.ZipCode = doctor.ZipCode;
                    dbDoctor.Address = doctor.Address;
                    dbDoctor.County = doctor.County;
                    dbDoctor.Email = doctor.Email;
                    dbDoctor.Phone = doctor.Phone;
                    dbDoctor.UpdatedBy = sessionContext.LoginName;
                    _doctorRepository.Update(dbDoctor);
                    _unitOfWork.Commit();
                    _logger.LogInformation("Updating doctor completed");
                }
                catch //(Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw;
                }
            }
        }
        void IDoctorService.DeleteDoctor(DoctorBO doctor)
        {
            var p = _mapper.Map<Repository.Models.Doctor>(doctor);
            try
            {
                _doctorRepository.Delete(p);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                if (ex.Errors.Count > 0 && ex.Errors[0].Number == 547)
                    throw new ServiceException(ClientSideErrors.DOCTOR_IS_ASSOCIATED);
                throw ex;
            }
            catch
            {
                throw;
            }
        }
    }
}
