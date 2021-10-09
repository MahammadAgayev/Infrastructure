using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace StorageCore.Domain.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository UserRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        IRoleRepository RoleRepository { get; }

        DbTransaction CreateTransaction();
        Task<DbTransaction> CreateTransactionAsync();
    }
}