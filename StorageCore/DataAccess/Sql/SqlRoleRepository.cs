using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Domain.Abstract;
using StorageCore.Domain.Entities;

namespace StorageCore.DataAccess.Sql
{
    public class SqlRoleRepository : IRoleRepository
    {
        private readonly IAsyncDbHelper _asyncDbHelper;
        private readonly IAsyncOrmDbHelper _asyncOrmDbHelper;

        public SqlRoleRepository(IAsyncDbHelper asyncDbHelper, IAsyncOrmDbHelper asyncOrmDbHelper)
        {
            _asyncDbHelper = asyncDbHelper;
            _asyncOrmDbHelper = asyncOrmDbHelper;
        }

        public async Task CreateAsync(Role role, DbTransaction transaction)
        {
            var @params = new Dictionary<string, object>
            {
                { "name", role.Name },
                { "NormalizedName", role.NormalizedName }
            };

            role.Id = await _asyncOrmDbHelper.Insert("Roles", @params, transaction);
        }

        public async Task<Role> GetAsync(string normalizedrolename)
        {
            return (await _asyncDbHelper.GetDataAsync("select * from roles where normalizedname = @rolename", Mapper.MapToRole,
                _asyncDbHelper.CreateParameter("rolename", normalizedrolename, System.Data.DbType.String))).FirstOrDefault();
        }

        public async Task<Role> GetAsync(int id)
        {
            return (await _asyncDbHelper.GetDataAsync("select * from roles where id = @id", Mapper.MapToRole,
                _asyncDbHelper.CreateParameter("id", id, System.Data.DbType.Int32))).FirstOrDefault();
        }
    }
}