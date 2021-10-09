using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace StorageCore.DbHelper.Abstraction
{
    public interface ISyncDbHelper
    {
        DbParameter CreateParameter(string name, object value, DbType type, ParameterDirection direction = ParameterDirection.Input);

        int ExecuteNonQuery(string query, IDictionary<string, object> parameters = null);

        int ExecuteNonQuery(string query, DbTransaction tx, IDictionary<string, object> parameters = null);

        int ExecuteNonQuery(string query, params DbParameter[] parameters);

        int ExecuteNonQuery(string query, DbTransaction tx, params DbParameter[] parameters);

        object ExecuteScalar(string query, IDictionary<string, object> parameters = null);

        object ExecuteScalar(string query, DbTransaction tx, IDictionary<string, object> parameters = null);

        object ExecuteScalar(string query, params DbParameter[] parameters);

        object ExecuteScalar(string query, DbTransaction tx, params DbParameter[] parameters);

        IList<T> GetData<T>(string query, Func<IDataReader, T> entityReader, IDictionary<string, object> parameters = null);

        IList<T> GetData<T>(string query, Func<IDataReader, T> entityReader, params DbParameter[] parameters);

        T GetScalar<T>(string query, IDictionary<string, object> parameters = null);

        T GetScalar<T>(string query, params DbParameter[] parameters);

        DbTransaction GetDbTransaction();
    }
}