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

        private readonly IAsyncOrmDbHelper _asyncOrmDbHelper;
        private readonly ISyncOrmDbHelper _syncOrmDbHelper;


        public SqlUnitOfWork(IAsyncDbHelper asyncDbHelper, ISyncDbHelper syncDbHelper, IAsyncOrmDbHelper asyncOrmDbHelper, ISyncOrmDbHelper syncOrmDbHelper)
        {
            _asyncDbHelper = asyncDbHelper;
            _syncDbHelper = syncDbHelper;

            _asyncOrmDbHelper = asyncOrmDbHelper;
            _syncOrmDbHelper = syncOrmDbHelper;
        }


        public IAccountRepository UserRepository => new SqlAccountRepository(_asyncOrmDbHelper, _asyncDbHelper);
        public IUserRoleRepository UserRoleRepository => new SqlUserRoleRepository(_asyncDbHelper, _asyncOrmDbHelper);
        public IRoleRepository RoleRepository => new SqlRoleRepository(_asyncDbHelper, _asyncOrmDbHelper);

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