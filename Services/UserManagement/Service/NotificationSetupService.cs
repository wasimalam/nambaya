using AutoMapper;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using UserManagement.Contracts.Interfaces;
using UserManagement.Contracts.Models;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement
{
    public class NotificationSetupService : BaseService, INotificationSetupService
    {
        private readonly INotificationEventParamRepository _notificationEventParamRepository;
        private readonly INotificationEventTypeRepository _notificationEventTypeRepository;
        private readonly INotificationTemplateRepository _notificationTemplateRepository;
        private readonly ILookupService _lookupsService;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationSetupService> _logger;

        public NotificationSetupService(IServiceProvider serviceProvider, IMapper mapper) : base(serviceProvider)
        {
            _notificationTemplateRepository = _serviceProvider.GetRequiredService<INotificationTemplateRepository>();
            _notificationEventParamRepository = _serviceProvider.GetRequiredService<INotificationEventParamRepository>();
            _notificationEventTypeRepository = _serviceProvider.GetRequiredService<INotificationEventTypeRepository>();
            _lookupsService = _serviceProvider.GetRequiredService<ILookupService>();
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
            _logger = serviceProvider.GetRequiredService<ILogger<NotificationSetupService>>();
        }
        public long AddTemplate(NotificationTemplateBO notificationTemplateBO)
        {
            _logger.LogInformation("AddTemplate: Adding template started");
            var p = _mapper.Map<NotificationTemplate>(notificationTemplateBO);
            _notificationTemplateRepository.Insert(p);
            _logger.LogInformation("AddTemplate: Adding template completed");

            return p.ID;
        }
        public NotificationTemplateBO GetNotificationTemplate(long id)
        {
            _logger.LogInformation($"GetNotificationTemplate: against id {id}");
            return _mapper.Map<NotificationTemplateBO>(_notificationTemplateRepository.GetByID(id));
        }
        public PagedResults<NotificationTemplateBO> GetNotificationTemplates(int limit, int offset, string orderby, string param)
        {
            _logger.LogInformation($"GetNotificationTemplates: limit {limit} offset {offset} orderby {orderby} param {param}");
            PagedResults<NotificationTemplateBO> pg = new PagedResults<NotificationTemplateBO>();
            IFilter filter = null;
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                var filterlist = expressions.Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
            }
            else
            {
                _logger.LogDebug("GetNotificationTemplates: Param is empty");
            }
            var pdb = _notificationTemplateRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<NotificationTemplateBO>(p)).ToList();
            _logger.LogInformation($"GetNotificationTemplates: Completed");

            return pg;
        }
        public void UpdateTemplate(NotificationTemplateBO notificationTemplateBO)
        {
            _logger.LogInformation("Updat template started");
            var notificationTemplate = _notificationTemplateRepository.GetByID(notificationTemplateBO.ID);
            notificationTemplate.Code = notificationTemplateBO.Code;
            notificationTemplate.TemplateTypeID = notificationTemplateBO.TemplateTypeID;
            notificationTemplate.EventTypeID = notificationTemplateBO.EventTypeID;
            notificationTemplate.Subject = notificationTemplateBO.Subject;
            notificationTemplate.Message = notificationTemplateBO.Message;
            notificationTemplate.IsActive = notificationTemplateBO.IsActive;
            notificationTemplate.UpdatedBy = notificationTemplateBO.UpdatedBy;
            _notificationTemplateRepository.Update(notificationTemplate);
            _logger.LogInformation("Update template completed");
        }
        public IEnumerable<NotificationEventTypeBO> GetNotificationTypesByEvent(long eventid)
        {
            _logger.LogDebug($"GetNotificationTypesByEvent: event id {eventid}");
            return _notificationEventTypeRepository.GetByEventType(eventid).Select(p => _mapper.Map<NotificationEventTypeBO>(p));
        }
        public IEnumerable<NotificationEventParamBO> GetNotificationParamsByEvent(long eventid)
        {
            _logger.LogInformation($"GetNotificationParamsByEvent: event id {eventid}");
            return _notificationEventParamRepository.GetByEventType(eventid).Select(p => _mapper.Map<NotificationEventParamBO>(p));
        }

        public IEnumerable<LookupsBO> GetNotificationEventTypes()
        {
            _logger.LogInformation($"Getting Notifications event types");

            return _lookupsService.GetItems("NOTIFICATIONEVENTTYPE");
        }

        public IEnumerable<LookupsBO> GetNotificationTemplateTypes()
        {
            _logger.LogInformation($"Getting Notifications template types");
            return _lookupsService.GetItems("NOTIFICATIONTEMPLATETYPE");
        }

        public NotificationTemplateBO GetNotificationTemplateByEventTemplate(long eventtypeid, long templatetypeid)
        {
            _logger.LogInformation($"GetNotificationTemplateByEventTemplate: eventtypeid {eventtypeid} templatetypeid {templatetypeid}");
            return _mapper.Map<NotificationTemplateBO>(_notificationTemplateRepository.GetNotificationTemplateByEventTemplate(eventtypeid, templatetypeid));
        }
    }
}