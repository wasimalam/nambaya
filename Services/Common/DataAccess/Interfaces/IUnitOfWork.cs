using System;

namespace Common.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }
        //IDbConnection Session { get; }
        //IDbTransaction Transaction { get; }
        IUnitOfWork BeginRequired();
        IUnitOfWork BeginRequiresNew();
        void Commit();
        void Rollback();
    }
}
