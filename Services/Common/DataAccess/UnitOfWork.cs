using Common.DataAccess.Interfaces;
using System;
using System.Transactions;

namespace Common.DataAccess
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly Guid _id = Guid.Empty;

        public UnitOfWork()
        {
            _id = Guid.NewGuid();
            this.Scope = new TransactionScope(TransactionScopeOption.Required);
        }

        private TransactionScope Scope { get; set; }

        Guid IUnitOfWork.Id
        {
            get { return _id; }
        }

        public IUnitOfWork BeginRequired()
        {
            if (this.Scope == null)
            {
                this.Scope = new TransactionScope(TransactionScopeOption.Required);
            }
            return this;
        }

        public IUnitOfWork BeginRequiresNew()
        {
            if (this.Scope == null)
            {
                this.Scope = new TransactionScope(TransactionScopeOption.RequiresNew);
            }
            return this;
        }

        public void Commit()
        {
            if (this.Scope != null)
            {
                this.Scope.Complete();
            }
            Dispose();
        }

        public void Rollback()
        {
            if (this.Scope != null)
            {
                this.Scope.Dispose();
            }
            Dispose();
        }

        public void Dispose()
        {
            if (Scope != null)
            {
                Scope.Dispose();
                Scope = null;
            }
        }
    }
}
