using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using StorageCore.DbHelper;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Domain.Abstract;
using StorageCore.Domain.Entities;

namespace StorageCore.DataAccess.Sql
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly IDbContext _context;
        private readonly IAsyncDbHelper _asyncDbHelper;
        private readonly IQueryHelper _queryHelper;

        public SqlUserRepository(IDbContext dbContext, IAsyncDbHelper asyncDbHelper, IQueryHelper queryHelper)
        {
            _context = dbContext;
            _asyncDbHelper = asyncDbHelper;
            _queryHelper = queryHelper;
        }

        public async Task CreateAsync(User user)
        {
            var @params = new Dictionary<string, object>
            {
                { nameof(User.Email), user.Email },
                { nameof(User.PhoneNumber), user.PhoneNumber },
                { nameof(User.PhoneNumberConfirmed), user.PhoneNumberConfirmed },
                { nameof(User.EmailConfirmed), user.EmailConfirmed },
                { nameof(User.NormalizedEmail), user.NormalizedEmail },
                { nameof(User.PasswordHash), user.PasswordHash },
                { nameof(User.Created), user.Created },
                { nameof(User.Updated), user.Updated }
            };

            string insertQuery = _queryHelper.GetInsertQuery("Users", @params);

            string query = $"{insertQuery} SELECT SCOPE_IDENTITY()";

            user.Id = Convert.ToInt32(await _asyncDbHelper.ExecuteScalarAsync(query, _context.DbTransaction, @params));
        }

        public async Task UpdateAsync(User user)
        {
            var @params = new Dictionary<string, object>
            {
                { nameof(User.Id), user.Id },
                { nameof(User.Email), user.Email },
                { nameof(User.PhoneNumber), user.PhoneNumber },
                { nameof(User.PhoneNumberConfirmed), user.PhoneNumberConfirmed },
                { nameof(User.EmailConfirmed), user.EmailConfirmed },
                { nameof(User.NormalizedEmail), user.NormalizedEmail },
                { nameof(User.PasswordHash), user.PasswordHash },
                { nameof(User.Created), user.Created },
                { nameof(User.Updated), user.Updated }
            };

            var query = _queryHelper.GetUpdateQuery("users", @params, new Filter("Id", Comparison.Equal));

            await _asyncDbHelper.ExecuteNonQueryAsync(query, _asyncDbHelper.CreateParameter("Id", user.Id, DbType.Int32));
        }

        public async Task DeleteAsync(User user)
        {
            await _asyncDbHelper.ExecuteNonQueryAsync("delete from users where id = @id", _context.DbTransaction,
                _asyncDbHelper.CreateParameter("id", user.Id, DbType.Int32));
        }

        public async Task<User> GetAsync(int id)
        {
            return (await _asyncDbHelper.GetDataAsync("select * from users where id = @id", Mapper.MapToUser,
                _asyncDbHelper.CreateParameter("id", id, DbType.Int32))).FirstOrDefault();
        }

        public async Task<IList<User>> GetAsync()
        {
            return await _asyncDbHelper.GetDataAsync("select * from users", Mapper.MapToUser);
        }

        public async Task<User> GetByEmailAsync(string normalizedemail)
        {
            return (await _asyncDbHelper.GetDataAsync("select * from users where normalizedemail = @email", Mapper.MapToUser,
                _asyncDbHelper.CreateParameter("email", normalizedemail, DbType.String))).FirstOrDefault();
        }

        public async Task<User> GetByPhoneAsync(string phonenumber)
        {
            return (await _asyncDbHelper.GetDataAsync("select * from users where phonenumber = @phone", Mapper.MapToUser,
                _asyncDbHelper.CreateParameter("phone", phonenumber, DbType.String))).FirstOrDefault();
        }
    }
}