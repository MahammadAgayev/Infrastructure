using System;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Domain.Abstract;

namespace StorageCore.DataAccess.Sql
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        private readonly IAsyncDbHelper _asyncDbHelper;
        private readonly IQueryHelper _queryHelper;

        private DbContext _dbContext = new DbContext();

        public SqlUnitOfWork(IAsyncDbHelper asyncDbHelper, IQueryHelper queryHelper)
        {
            _asyncDbHelper = asyncDbHelper;
            _queryHelper = queryHelper;
        }


        public IUserRepository UserRepository => new SqlUserRepository(_dbContext, _asyncDbHelper, _queryHelper);
        public IUserRoleRepository UserRoleRepository => new SqlUserRoleRepository(_dbContext, _asyncDbHelper, _queryHelper);
        public IRoleRepository RoleRepository => new SqlRoleRepository(_dbContext, _asyncDbHelper, _queryHelper);

        public void Commit()
        {
            _dbContext.DbTransaction.Value.Commit();
            _dbContext.DbTransaction.Value.Dispose();

            _dbContext.SetTransaction(null);
        }

        public void CreateTransaction()
        {
            if(_dbContext.DbTransaction.IsValueCreated)
            {
                throw new InvalidOperationException("transaction already created for this context");
            }

            var tx = _asyncDbHelper.GetDbTransaction().GetAwaiter().GetResult();

            _dbContext.SetTransaction(tx);
        }

        public void Rollback()
        {
            _dbContext.DbTransaction.Value.Rollback();
            _dbContext.DbTransaction.Value.Dispose();

            _dbContext.SetTransaction(null);
        }
    }
}
