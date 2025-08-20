using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using UserManagement.Contracts.Interfaces;
using UserManagement.Contracts.Models;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private IRoleService _roleService;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }
        [HttpGet("{applicationCode}")]
        public ActionResult<IEnumerable<RoleBO>> Get(string applicationCode)
        {
            var ph = _roleService.GetRoles(applicationCode).ToArray();
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("loginname/{loginname}")]
        public ActionResult<RoleBO> GetByLoginName(string loginname)
        {
            var ph = _roleService.GetRoleByLoginName(loginname);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
    }
}
