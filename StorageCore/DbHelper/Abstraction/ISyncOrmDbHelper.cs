using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace StorageCore.DbHelper.Abstraction
{
    public interface ISyncOrmDbHelper
    {
        int Insert(string tablename, IDictionary<string, object> parametres, DbTransaction tx);
        void Update(string tablename, IDictionary<string, object> parametres, DbTransaction tx, params Filter[] filters);

        IList<T> SimpleGet<T>(string tablename, string[] columns, Func<IDataReader, T> entityReader, params Filter[] filters);
        IList<T> JoinedGet<T>(string tablename, string[] columns, Join[] joins, Func<IDataReader, T> entityReader, params Filter[] filters);
    }
}