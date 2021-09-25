using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorageCore.DataAccess.Sql;
using StorageCore.DbHelper;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Domain.Abstract;

namespace ApiTemplate.Extensions
{
    public static class DbExtensions
    {
        public static void AddDb(this IServiceCollection services, IConfiguration config)
        {
            string cs = config.GetConnectionString("Default");

            services.Configure<SqlDbOptions>(x =>
            {
                x.ConnectionString = cs;
            });

            services.AddTransient<IQueryHelper, SqlQueryHelper>();
            services.AddTransient<IAsyncDbHelper, SqlAsyncDbHelper>();
            services.AddTransient<ISyncDbHelper, SqlSyncDbHelper>();

            services.AddTransient<IUnitOfWork, SqlUnitOfWork>();
        }
    }
}
