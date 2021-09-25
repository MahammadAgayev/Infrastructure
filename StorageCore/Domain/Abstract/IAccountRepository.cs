using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using StorageCore.Domain.Entities;

namespace StorageCore.Domain.Abstract
{
    public interface IAccountRepository
    {
        Task CreateAsync(Account user, DbTransaction transaction);
        Task UpdateAsync(Account user, DbTransaction transaction);
        Task DeleteAsync(Account user, DbTransaction transaction);

        Task<Account> GetAsync(int id);
        Task<Account> GetByPhoneAsync(string phonenumber);
        Task<Account> GetByEmailAsync(string email);

        Task<IList<Account>> GetAsync();
    }
}