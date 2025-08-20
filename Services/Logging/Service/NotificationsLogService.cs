using AutoMapper;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services;
using Logging.Contracts.Interfaces;
using Logging.Contracts.Models;
using Logging.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Logging.Service
{
    public class NotificationsLogService : BaseService, INotificationsLogService
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IMapper _mapper;
        public NotificationsLogService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _notificationsRepository = serviceProvider.GetRequiredService<INotificationsRepository>();
        }
        public PagedResults<NotificationsBO> GetLogs(int limit, int offset, string orderby, string param)
        {
            var sessionContext = GetSessionContext();
            if (sessionContext.RoleCode != RoleCodes.NambayaUser)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            PagedResults<NotificationsBO> pg = new PagedResults<NotificationsBO>();
            IFilter filter = null;
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                var filterlist = expressions.Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
            }
            if (string.IsNullOrWhiteSpace(orderby))
                orderby = "TimeStamp Asc";

            var pdb = _notificationsRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<NotificationsBO>(p)).ToList();
            return pg;
        }
        public void ExecuteInsert(NotificationsBO logObj)
        {
            _notificationsRepository.Insert(_mapper.Map<Repository.Models.Notifications>(logObj));
        }
    }
}
