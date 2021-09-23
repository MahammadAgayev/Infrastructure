using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorageCore.DataAccess.Sql;
using StorageCore.DbHelper;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Domain.Abstract;

namespace ApiTemplate.Extensions
{
    public static class AddDbExtensions
    {
        public static void AddDb(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<SqlDbOptions>(config.GetSection("SqlOptions"));

            services.AddTransient<IQueryHelper, SqlQueryHelper>();
            services.AddTransient<IAsyncDbHelper, SqlAsyncDbHelper>();
            services.AddTransient<ISyncDbHelper, SqlSyncDbHelper>();

            services.AddTransient<IUnitOfWork, SqlUnitOfWork>();
        }
    }
}
