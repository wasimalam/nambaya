using Common.DataAccess.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace Common.DataAccess
{
    public sealed class DatabaseSession : IDatabaseSession, IDisposable
    {
        private IDbConnection _session;
        private readonly IConfiguration _configuration;

        public DatabaseSession(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IDbConnection Session
        {
            get
            {
                if (_session == null)
                {
                    _session = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                    _session.Open();
                    ConnectionCount++;
                }
                else if (_session.State == ConnectionState.Closed || _session.State == ConnectionState.Broken)
                {
                    ConnectionCount++;
                    _session.Open();
                }
                return _session;
            }
        }

        public static int ConnectionCount { get; set; } = 0;

        public void Dispose()
        {
            if (_session != null && _session.State == ConnectionState.Open)
            {
                _session.Close();
                ConnectionCount--;
                _session.Dispose();
            }
            else if (_session != null && _session.State == ConnectionState.Closed)
            {
                _session.Dispose();
            }
        }
    }
}
