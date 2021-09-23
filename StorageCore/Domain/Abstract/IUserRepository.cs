using System.Collections.Generic;
using System.Threading.Tasks;
using StorageCore.Domain.Entities;

namespace StorageCore.Domain.Abstract
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);

        Task<User> GetAsync(int id);
        Task<User> GetByPhoneAsync(string phonenumber);
        Task<User> GetByEmailAsync(string email);

        Task<IList<User>> GetAsync();
    }
}