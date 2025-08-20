using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Contracts.Interfaces;
using UserManagement.Contracts.Models;
using UserManagement.Repository.Interfaces;

namespace UserManagement.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IServiceProvider serviceProvider)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _roleRepository = serviceProvider.GetRequiredService<IRoleRepository>();
            _logger = serviceProvider.GetRequiredService<ILogger<RoleService>>();
        }
        RoleBO IRoleService.GetRoleByLoginName(string loginname)
        {
            _logger.LogInformation($"GetRoleByLoginName: login name {loginname}");
            return _mapper.Map<RoleBO>(_roleRepository.GetByLoginName(loginname));
        }
        List<RoleBO> IRoleService.GetRoles(string applicationcode)
        {
            _logger.LogInformation($"GetRoles: applicationcode {applicationcode}");
            return _roleRepository.GetByApplicationCode(applicationcode).Select(p => _mapper.Map<RoleBO>(p)).ToList();
        }
    }
}
