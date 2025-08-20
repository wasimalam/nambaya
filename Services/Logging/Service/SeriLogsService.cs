using AutoMapper;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services;
using Logging.Contracts.Interfaces;
using Logging.Contracts.Models;
using Logging.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using System;
using System.Linq;

namespace Logging.Service
{
    public class SeriLogsService : BaseService, ILoggingService, ILoggingInsertService
    {
        private readonly ISerilogsRepository _serilogsRepository;
        private readonly IMapper _mapper;
        public SeriLogsService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _serilogsRepository = serviceProvider.GetRequiredService<ISerilogsRepository>();
        }
        public PagedResults<SeriLogsBO> GetLogs(int limit, int offset, string orderby, string param)
        {
            var sessionContext = GetSessionContext();            
            if (sessionContext.RoleCode != RoleCodes.NambayaUser)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            PagedResults<SeriLogsBO> pg = new PagedResults<SeriLogsBO>();
            IFilter filter = null;
            if (!string.IsNullOrWhiteSpace(param))
            {
                var expressions = Expression.StringToExpressions(param);
                var filterlist = expressions.Select(p => new Filter(p.Property, p.Operation, p.Value)).ToList();
                filter = new ANDFilter(filterlist.Select(p => p as IFilter).ToList());
            }
            if (string.IsNullOrWhiteSpace(orderby))
                orderby = "TimeStamp Asc";

            var pdb = _serilogsRepository.GetAllPages(limit, offset, orderby, filter);
            pg.TotalCount = pdb.TotalCount;
            pg.PageSize = pdb.PageSize;
            pg.Data = pdb.Data.Select(p => _mapper.Map<SeriLogsBO>(p)).ToList();
            return pg;
        }
        public void InsertRecord(object logObj)
        {
            LogEvent logEvent = logObj as LogEvent;
            _serilogsRepository.Insert(new Repository.Models.SeriLogs()
            {
                ApplicationName = logEvent.Properties.ContainsKey(LoggingConstants.Application) ? (logEvent.Properties[LoggingConstants.Application] as ScalarValue).Value.ToString() : null,
                CorrelationId = logEvent.Properties.ContainsKey(LoggingConstants.CorrelationId) ? (logEvent.Properties[LoggingConstants.CorrelationId] as ScalarValue).Value.ToString() : null,
                Level = logEvent.Level.ToString(),
                Exception = logEvent.Exception?.ToString(),
                Message = logEvent.RenderMessage(),
                TimeStamp = logEvent.Timestamp.UtcDateTime,
            });
        }
    }
}
