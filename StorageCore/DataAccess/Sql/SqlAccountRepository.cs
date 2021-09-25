using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using StorageCore.DbHelper;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Domain.Abstract;
using StorageCore.Domain.Entities;

namespace StorageCore.DataAccess.Sql
{
    public class SqlAccountRepository : IAccountRepository
    {
        private readonly IAsyncDbHelper _asyncDbHelper;
        private readonly IQueryHelper _queryHelper;

        public SqlAccountRepository(IAsyncDbHelper asyncDbHelper, IQueryHelper queryHelper)
        {
            _asyncDbHelper = asyncDbHelper;
            _queryHelper = queryHelper;
        }

        public async Task CreateAsync(Account user, DbTransaction transaction)
        {
            var @params = new Dictionary<string, object>
            {
                { nameof(Account.Email), user.Email },
                { nameof(Account.PhoneNumber), user.PhoneNumber },
                { nameof(Account.PhoneNumberConfirmed), user.PhoneNumberConfirmed },
                { nameof(Account.EmailConfirmed), user.EmailConfirmed },
                { nameof(Account.NormalizedEmail), user.NormalizedEmail },
                { nameof(Account.PasswordHash), user.PasswordHash },
                { nameof(Account.Created), user.Created },
                { nameof(Account.Updated), user.Updated }
            };

            string insertQuery = _queryHelper.GetInsertQuery("Accounts", @params);

            string query = $"{insertQuery} SELECT SCOPE_IDENTITY()";

            user.Id = Convert.ToInt32(await _asyncDbHelper.ExecuteScalarAsync(query, transaction, @params));
        }

        public async Task UpdateAsync(Account user, DbTransaction transaction)
        {
            var @params = new Dictionary<string, object>
            {
                { nameof(Account.Id), user.Id },
                { nameof(Account.Email), user.Email },
                { nameof(Account.PhoneNumber), user.PhoneNumber },
                { nameof(Account.PhoneNumberConfirmed), user.PhoneNumberConfirmed },
                { nameof(Account.EmailConfirmed), user.EmailConfirmed },
                { nameof(Account.NormalizedEmail), user.NormalizedEmail },
                { nameof(Account.PasswordHash), user.PasswordHash },
                { nameof(Account.Created), user.Created },
                { nameof(Account.Updated), user.Updated }
            };

            var query = _queryHelper.GetUpdateQuery("Accounts", @params, new Filter("Id", Comparison.Equal));

            await _asyncDbHelper.ExecuteNonQueryAsync(query, transaction, @params);
        }

        public async Task DeleteAsync(Account user, DbTransaction transaction)
        {
            await _asyncDbHelper.ExecuteNonQueryAsync("delete from Accounts where id = @id", transaction,
                _asyncDbHelper.CreateParameter("id", user.Id, DbType.Int32));
        }

        public async Task<Account> GetAsync(int id)
        {
            return (await _asyncDbHelper.GetDataAsync("select * from users where id = @id", Mapper.MapToUser,
                _asyncDbHelper.CreateParameter("id", id, DbType.Int32))).FirstOrDefault();
        }

        public async Task<IList<Account>> GetAsync()
        {
            return await _asyncDbHelper.GetDataAsync("select * from Accounts", Mapper.MapToUser);
        }

        public async Task<Account> GetByEmailAsync(string normalizedemail)
        {
            return (await _asyncDbHelper.GetDataAsync("select * from Accounts where normalizedemail = @email", Mapper.MapToUser,
                _asyncDbHelper.CreateParameter("email", normalizedemail, DbType.String))).FirstOrDefault();
        }

        public async Task<Account> GetByPhoneAsync(string phonenumber)
        {
            return (await _asyncDbHelper.GetDataAsync("select * from Accounts where phonenumber = @phone", Mapper.MapToUser,
                _asyncDbHelper.CreateParameter("phone", phonenumber, DbType.String))).FirstOrDefault();
        }
    }
}