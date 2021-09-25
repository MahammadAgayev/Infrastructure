using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using StorageCore.Domain.Entities;

namespace StorageCore.Domain.Abstract
{
    public interface IUserRoleRepository
    {
        Task CreateAsync(AccountRole userRole, DbTransaction transaction);

        Task<IList<Role>> GetUserRolesAsync(Account user);
        Task<IList<Account>> GetUsersByRoleAsync(string rolename);

        Task DeleteAsync(Account user, string rolename, DbTransaction transaction);
    }
}