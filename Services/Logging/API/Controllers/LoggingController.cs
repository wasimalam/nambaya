using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Logging.Contracts.Interfaces;
using Logging.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Logging.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;
        private ILoggingService _loggingService;
        private INotificationsLogService _notificationsLogService;
        private readonly IConfiguration _configuration;
        public LoggingController(ILogger<LoggingController> logger, ILoggingService loggingService,
            INotificationsLogService notificationsLogService, IConfiguration configuration)
        {
            _logger = logger;
            _loggingService = loggingService;
            _notificationsLogService = notificationsLogService;
            _configuration = configuration;
        }
        [HttpGet]
        public ActionResult<PagedResults<SeriLogsBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _loggingService.GetLogs(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("{action}")]
        public ActionResult<PagedResults<NotificationsBO>> GetNotificationsLog(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _notificationsLogService.GetLogs(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpPost("{action}")]
        public ActionResult LogNotifications(NotificationsBO notificationsBO)
        {
            try
            {
                _notificationsLogService.ExecuteInsert(notificationsBO);
                return Ok();
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [HttpGet("{action}")]
        public ActionResult<string[]> GetLogAllowedUserDomains()
        {
            try
            {
                string[] allowedUserDomains = _configuration.GetSection("LogViewAllowedUserDomains").Get<string[]>();
                return Ok(allowedUserDomains);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
    }
}
