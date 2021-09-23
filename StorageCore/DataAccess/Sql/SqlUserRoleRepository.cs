using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Domain.Abstract;
using StorageCore.Domain.Entities;

namespace StorageCore.DataAccess.Sql
{
    public class SqlUserRoleRepository : IUserRoleRepository
    {
        private readonly IDbContext _dbContext;
        private readonly IAsyncDbHelper _asyncDbHelper;
        private readonly IQueryHelper _queryHelper;

        public SqlUserRoleRepository(IDbContext dbContext, IAsyncDbHelper asyncDbHelper, IQueryHelper queryHelper)
        {
            _dbContext = dbContext;
            _asyncDbHelper = asyncDbHelper;
            _queryHelper = queryHelper;
        }

        public async Task CreateAsync(UserRole userRole)
        {
            var @params = new Dictionary<string, object>
             {
                 { "roleid" , userRole.Role.Id },
                 { "userid", userRole.User.Id }
             };

            string insertQuery = _queryHelper.GetInsertQuery("userroles", @params);

            string query = $"{insertQuery} SELECT SCOPE_IDENTITY()";

            userRole.Id = Convert.ToInt32(await _asyncDbHelper.ExecuteScalarAsync(query, _dbContext.DbTransaction, @params));
        }

        public async Task DeleteAsync(User user, string rolename)
        {
            await _asyncDbHelper.ExecuteNonQueryAsync("delete from userroles where userid = @userid and normalizedrolename = @rolename",
                _dbContext.DbTransaction, _asyncDbHelper.CreateParameter("userid", user.Id, DbType.Int32),
                _asyncDbHelper.CreateParameter("rolename", rolename, DbType.String));
        }

        public async Task<IList<Role>> GetUserRolesAsync(User user)
        {
            return await _asyncDbHelper.GetDataAsync(@"select r.* from userroles ur 
                          left join roles r on r.id == ur.roleid
                         where ur.userid = @userid", Mapper.MapToRole,
                _asyncDbHelper.CreateParameter("userid", user.Id, DbType.Int32));
        }

        public async Task<IList<User>> GetUsersByRoleAsync(string rolename)
        {
            return await _asyncDbHelper.GetDataAsync(@"select u.* from userroles ur 
                          inner join users u on u.id = ur.userid
                          inner join roles r on r.id = ur.roleid
                         where  r.normalizedrolename = @rolename", Mapper.MapToUser,
                        _asyncDbHelper.CreateParameter("rolename", rolename, DbType.String));
        }
    }
}