using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace StorageCore.DbHelper.Abstraction
{
    public interface IAsyncOrmDbHelper
    {
        Task<int> Insert(string tablename, IDictionary<string, object> parametres, DbTransaction tx);
        Task Update(string tablename, IDictionary<string, object> parametres, DbTransaction tx, params Filter[] filters);

        Task<IList<T>> SimpleGet<T>(string tablename, string[] columns, Func<IDataReader, T> entityReader, params Filter[] filters);
        Task<IList<T>> JoinedGet<T>(string tablename, string[] columns, Join[] joins, Func<IDataReader, T> entityReader, params Filter[] filters);
    }
}