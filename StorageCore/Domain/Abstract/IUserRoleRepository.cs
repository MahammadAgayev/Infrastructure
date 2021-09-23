using System.Collections.Generic;
using System.Threading.Tasks;
using StorageCore.Domain.Entities;

namespace StorageCore.Domain.Abstract
{
    public interface IUserRoleRepository
    {
        Task CreateAsync(UserRole userRole);

        Task<IList<Role>> GetUserRolesAsync(User user);
        Task<IList<User>> GetUsersByRoleAsync(string rolename);

        Task DeleteAsync(User user, string rolename);
    }
}