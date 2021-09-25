using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace StorageCore.Domain.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        public IAccountRepository UserRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }
        public IRoleRepository RoleRepository { get; }

        DbTransaction CreateTransaction();
        Task<DbTransaction> CreateTransactionAsync();
    }
}