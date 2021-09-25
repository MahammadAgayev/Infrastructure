using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Domain.Abstract;
using StorageCore.Domain.Entities;

namespace StorageCore.DataAccess.Sql
{
    public class SqlUserRoleRepository : IUserRoleRepository
    {
        private readonly IAsyncDbHelper _asyncDbHelper;
        private readonly IQueryHelper _queryHelper;

        public SqlUserRoleRepository(IAsyncDbHelper asyncDbHelper, IQueryHelper queryHelper)
        {
            _asyncDbHelper = asyncDbHelper;
            _queryHelper = queryHelper;
        }

        public async Task CreateAsync(AccountRole userRole, DbTransaction transaction)
        {
            var @params = new Dictionary<string, object>
             {
                 { "roleid" , userRole.Role.Id },
                 { "accountid", userRole.User.Id }
             };

            string insertQuery = _queryHelper.GetInsertQuery("AccountRoles", @params);

            string query = $"{insertQuery} SELECT SCOPE_IDENTITY()";

            userRole.Id = Convert.ToInt32(await _asyncDbHelper.ExecuteScalarAsync(query, transaction, @params));
        }

        public async Task DeleteAsync(Account user, string rolename, DbTransaction transaction)
        {
            await _asyncDbHelper.ExecuteNonQueryAsync("delete from AccountRoles where userid = @userid and normalizedrolename = @rolename",
                  transaction, _asyncDbHelper.CreateParameter("userid", user.Id, DbType.Int32),
                _asyncDbHelper.CreateParameter("rolename", rolename, DbType.String));
        }

        public async Task<IList<Role>> GetUserRolesAsync(Account user)
        {
            return await _asyncDbHelper.GetDataAsync(@"select r.* from AccountRoles ur 
                          left join roles r on r.id = ur.roleid
                         where ur.accountid = @userid", Mapper.MapToRole,
                _asyncDbHelper.CreateParameter("userid", user.Id, DbType.Int32));
        }

        public async Task<IList<Account>> GetUsersByRoleAsync(string rolename)
        {
            return await _asyncDbHelper.GetDataAsync(@"select u.* from AccountRoles ur 
                          inner join accounts u on u.id = ur.accountid
                          inner join roles r on r.id = ur.roleid
                         where  r.normalizedrolename = @rolename", Mapper.MapToUser,
                        _asyncDbHelper.CreateParameter("rolename", rolename, DbType.String));
        }
    }
}