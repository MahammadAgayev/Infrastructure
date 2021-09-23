using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Domain.Abstract;
using StorageCore.Domain.Entities;

namespace StorageCore.DataAccess.Sql
{
    public class SqlRoleRepository : IRoleRepository
    {
        private readonly IDbContext _dbcontext;
        private readonly IAsyncDbHelper _asyncDbHelper;
        private readonly IQueryHelper _queryHelper;

        public SqlRoleRepository(IDbContext dbContext, IAsyncDbHelper asyncDbHelper, IQueryHelper queryHelper)
        {
            _dbcontext = dbContext;
            _asyncDbHelper = asyncDbHelper;
            _queryHelper = queryHelper;
        }

        public async Task CreateAsync(Role role)
        {
            var @params = new Dictionary<string, object>
            {
                { "name", role.Name },
                { "normalizedname", role.NormalizedName }
            };

            string insertQuery = _queryHelper.GetInsertQuery("Roles", @params);

            string query = $"{insertQuery} SELECT SCOPE_IDENTITY()";

            role.Id = Convert.ToInt32(await _asyncDbHelper.ExecuteScalarAsync(query, _dbcontext.DbTransaction, @params));
        }

        public async Task<Role> GetAsync(string normalizedrolename)
        {
            return (await _asyncDbHelper.GetDataAsync("select * from roles where normalizedrolename = @rolename", Mapper.MapToRole,
                _asyncDbHelper.CreateParameter("rolename", normalizedrolename, System.Data.DbType.String))).FirstOrDefault();
        }

        public async Task<Role> GetAsync(int id)
        {
            return (await _asyncDbHelper.GetDataAsync("select * from roles where id = @id", Mapper.MapToRole,
                _asyncDbHelper.CreateParameter("id", id, System.Data.DbType.Int32))).FirstOrDefault();
        }
    }
}