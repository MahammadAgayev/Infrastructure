using System.Threading.Tasks;
using StorageCore.Domain.Entities;

namespace StorageCore.Domain.Abstract
{
    public interface IRoleRepository
    {
        Task CreateAsync(Role role);

        Task<Role> GetAsync(string rolename);
        Task<Role> GetAsync(int id);
    }
}
