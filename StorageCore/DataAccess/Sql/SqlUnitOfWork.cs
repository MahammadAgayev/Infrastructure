using System;
using System.Data.Common;
using System.Threading.Tasks;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Domain.Abstract;

namespace StorageCore.DataAccess.Sql
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        private readonly IAsyncDbHelper _asyncDbHelper;
        private readonly ISyncDbHelper _syncDbHelper;

        private readonly IQueryHelper _queryHelper;

        public SqlUnitOfWork(IAsyncDbHelper asyncDbHelper, ISyncDbHelper syncDbHelper, IQueryHelper queryHelper)
        {
            _asyncDbHelper = asyncDbHelper;
            _syncDbHelper = syncDbHelper;
            _queryHelper = queryHelper;
        }


        public IAccountRepository UserRepository => new SqlAccountRepository( _asyncDbHelper, _queryHelper);
        public IUserRoleRepository UserRoleRepository => new SqlUserRoleRepository(_asyncDbHelper, _queryHelper);
        public IRoleRepository RoleRepository => new SqlRoleRepository(_asyncDbHelper, _queryHelper);

        public DbTransaction CreateTransaction()
        {
            return _syncDbHelper.GetDbTransaction();
        }

        public async Task<DbTransaction> CreateTransactionAsync()
        {
            return await _asyncDbHelper.GetDbTransaction();
        }

        public void Dispose()
        {
        }
    }
}