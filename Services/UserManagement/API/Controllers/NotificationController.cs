using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using UserManagement.Contracts.Interfaces;
using UserManagement.Contracts.Models;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private INotificationSetupService _notificationSetupService;

        public NotificationController(ILogger<NotificationController> logger, INotificationSetupService notificationSetupService)
        {
            _logger = logger;
            _notificationSetupService = notificationSetupService;
        }
        [HttpGet("notificationtemplatetypes")]
        public ActionResult<IEnumerable<LookupsBO>> GetNotificationTemplateTypes()
        {
            var ph = _notificationSetupService.GetNotificationTemplateTypes();
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("notificationeventtypes")]
        public ActionResult<IEnumerable<LookupsBO>> GetNotificationEventTypes()
        {
            var ph = _notificationSetupService.GetNotificationEventTypes();
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("notificationtemplates")]
        public ActionResult<PagedResults<NotificationTemplateBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _notificationSetupService.GetNotificationTemplates(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TEMPLATE");
            }
            return Ok(ph);
        }
        [HttpGet("notificationtemplates/eventtype")]
        public ActionResult<NotificationTemplateBO> GetNotificationTemplateByEventTemplate(long eventtypeid, long templatetypeid)
        {
            var ph = _notificationSetupService.GetNotificationTemplateByEventTemplate(eventtypeid, templatetypeid);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TEMPLATE");
            }
            return Ok(ph);
        }
        [HttpGet("notificationtemplates/{id}")]
        public ActionResult<PagedResults<NotificationTemplateBO>> Get(long id)
        {
            var ph = _notificationSetupService.GetNotificationTemplate(id);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TEMPLATE");
            }
            return Ok(ph);
        }
        [HttpPost("notificationtemplates")]
        public ActionResult<NotificationTemplateBO> CreateTemplate([FromBody] NotificationTemplateBO notificationTemplateBO)
        {
            try
            {
                var id = _notificationSetupService.AddTemplate(notificationTemplateBO);
                return Ok(_notificationSetupService.GetNotificationTemplate(id));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [HttpPut("notificationtemplates")]
        public ActionResult<UserBO> UpdateTemplate([FromBody] NotificationTemplateBO notificationTemplateBO)
        {
            try
            {
                var ph = _notificationSetupService.GetNotificationTemplate(notificationTemplateBO.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _notificationSetupService.UpdateTemplate(notificationTemplateBO);
                return Ok(_notificationSetupService.GetNotificationTemplate(notificationTemplateBO.ID));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [HttpGet("notificationtypes/{eventtypeid}")]
        public ActionResult<IEnumerable<NotificationEventTypeBO>> GetNotificationTypesByEvent(long eventtypeid)
        {
            var ph = _notificationSetupService.GetNotificationTypesByEvent(eventtypeid);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TYPE");
            }
            return Ok(ph);
        }
        [HttpGet("eventparams/{eventtypeid}")]
        public ActionResult<IEnumerable<NotificationEventParamBO>> GetNotificationParamsByEvent(long eventtypeid)
        {
            var ph = _notificationSetupService.GetNotificationParamsByEvent(eventtypeid);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TYPE");
            }
            return Ok(ph);
        }
    }
}
