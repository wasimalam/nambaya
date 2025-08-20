using System.Data;

namespace Common.DataAccess.Interfaces
{
    public interface IDatabaseSession
    {
        IDbConnection Session { get; }
    }
}
