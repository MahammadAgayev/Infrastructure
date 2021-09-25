using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;


namespace StorageCore.DbHelper.Abstraction
{
    public interface IAsyncDbHelper
    {
        DbParameter CreateParameter(string name, object value, DbType type, ParameterDirection direction = ParameterDirection.Input);

        Task<int> ExecuteNonQueryAsync(string query, IDictionary<string, object> parameters = null);

        Task<int> ExecuteNonQueryAsync(string query, DbTransaction tx, IDictionary<string, object> parameters = null);

        Task<int> ExecuteNonQueryAsync(string query, params DbParameter[] parameters);

        Task<int> ExecuteNonQueryAsync(string query, DbTransaction tx, params DbParameter[] parameters);

        Task<object> ExecuteScalarAsync(string query, IDictionary<string, object> parameters = null);

        Task<object> ExecuteScalarAsync(string query, DbTransaction tx, IDictionary<string, object> parameters = null);

        Task<object> ExecuteScalarAsync(string query, params DbParameter[] parameters);

        Task<object> ExecuteScalarAsync(string query, DbTransaction tx, params DbParameter[] parameters);

        Task<IList<T>> GetDataAsync<T>(string query, Func<IDataReader, T> entityReader, IDictionary<string, object> parameters = null);

        Task<IList<T>> GetDataAsync<T>(string query, Func<IDataReader, T> entityReader, params DbParameter[] parameters);
        Task<IList<T>> GetDataAsync<T>(string query, DbTransaction tx, Func<IDataReader, T> entityReader, params DbParameter[] parameters);

        Task<T> GetScalarAsync<T>(string query, IDictionary<string, object> parameters = null);
        Task<T> GetScalarAsync<T>(string query, params DbParameter[] parameters);

        Task<DbTransaction> GetDbTransaction();
    }
}