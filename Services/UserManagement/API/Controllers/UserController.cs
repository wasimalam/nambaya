using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private IUserService _userService;
        private IAppCache _cache;
        public UserController(ILogger<UserController> logger, IUserService userService, IAppCache cache)
        {
            _logger = logger;
            _userService = userService;
            _cache = cache;
        }

        [HttpGet]
        public ActionResult<PagedResults<UserBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _userService.GetUsers(offset, limit, orderby, filter);
            if (ph == null)
            {
                return NotFound("NO_VALID_USER");
            }
            return Ok(ph);
        }
        [HttpGet("{id}")]
        public ActionResult<UserBO> Get(int id)
        {
            var ph = _userService.GetUserById(id);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("context/{id}")]
        public ActionResult<SessionContext> GetContext(int id)
        {
            var ph = _userService.GetUserSessionById(id);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("context/email/{loginname}")]
        public ActionResult<SessionContext> GetContext(string loginname)
        {
            var user = _userService.GetUserSessionByLoginId(loginname);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }
        [HttpGet("loginname/{loginname}")]
        public ActionResult<UserBO> GetByLoginName(string loginname)
        {
            var ph = _userService.GetUserByLoginName(loginname);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("{action}")]
        public ActionResult<List<UserBO>> GetByLoginNames([FromBody] string[] loginnames)
        {
            if (loginnames.Length == 0)
                return NotFound();
            var ph = _userService.GetUsersByLoginName(loginnames);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("{action}")]
        public ActionResult<List<UserBO>> GetByRole(string role, bool? isActive, bool? isLocked)
        {
            Func<List<UserBO>> dataGetter = () => _userService.GetUsersByRole(role, isActive, isLocked);
            var dataWithCaching = _cache.GetOrAdd($"{role}__{isActive}__{isLocked}", dataGetter);
            return dataWithCaching;
        }
        [HttpGet("settings/{loginname}")]
        public ActionResult<List<UserSettingBO>> GetSettings(string loginname)
        {
            var ph = _userService.GetUserSettings(loginname);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }

        [HttpPost("{action}")]
        public ActionResult<SessionContext> IsValidUser([FromBody] LoginRequest req)
        {
            try
            {
                var user = _userService.IsValidUser(req.LoginId, req.Password, true);
                if (user != null)
                    return Ok(user);
                return BadRequest(ClientSideErrors.INVALID_USER_ID_PASSWORD);
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
        [HttpPost("{action}")]
        public ActionResult<UserBO> Create([FromBody] BaseUserBO req)
        {
            try
            {
                var user = _userService.CreateUser(req);
                return Ok(user);
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
        [HttpPost("{action}")]
        public ActionResult<UserBO> Update([FromBody] BaseUserBO req)
        {
            try
            {
                _userService.UpdateUser(req);
                return Ok(_userService.GetUserByLoginName(req.LoginName));
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
        [HttpPost("{action}")]
        public ActionResult UpdateCredentials([FromBody] LoginRequest req)
        {
            try
            {
                _userService.UpdateUserCredentials(new BaseUserBO() { LoginName = req.LoginId, Password = req.Password });
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
        [HttpPost("settings/{loginname}")]
        public ActionResult UpdateSettings(string loginname, [FromBody] List<UserSettingBO> userSettings)
        {
            try
            {
                _userService.UpateUserSettings(loginname, userSettings);
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

        [HttpPost("{action}/{loginname}")]
        public ActionResult ChangeCredentials(string loginname, [FromBody] ChangeCredentialBO req)
        {
            try
            {
                _userService.ChangeCredentials(loginname, req);
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
    }
    public class RequestContext
    {
        public string UserLoginId { get; set; }
        public string ApplicationId { get; set; }
    }
    public class LoginRequest
    {
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string ApplicationId { get; set; }
    }
}
