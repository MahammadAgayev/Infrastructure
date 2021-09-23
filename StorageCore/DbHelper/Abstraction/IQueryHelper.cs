using System.Collections.Generic;
using System.Data.Common;

namespace StorageCore.DbHelper.Abstraction
{
    public interface IQueryHelper
    {
        string GetInsertQuery(string tablename, IDictionary<string, object> parametres);
        string GetUpdateQuery(string tablename, IDictionary<string, object> parametres, params Filter[] filters);

        string GetSimpleGetQuery(string tablename, string[] columns, IDictionary<string, object> parametres, params Filter[] filters);

        string GetJoinedGetQuery(string tablename, string[] columns, Join[] joins, IDictionary<string, object> parametres, params Filter[] filters);
    }
}