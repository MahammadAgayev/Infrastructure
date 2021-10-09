using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using StorageCore.DbHelper.Abstraction;

namespace StorageCore.DbHelper
{
    public class SqlSyncOrmDbHelper : ISyncOrmDbHelper
    {
        private readonly ISyncDbHelper _syncDbHelper;

        public SqlSyncOrmDbHelper(ISyncDbHelper syncDbHelper) => _syncDbHelper = syncDbHelper;

        public int Insert(string tablename, IDictionary<string, object> parametres, DbTransaction tx)
        {
            string columns = string.Join(',', parametres.Select(x => x.Key));
            string values = string.Join(',', parametres.Select(x => $"@{x.Key}"));

            string query = $"insert into {tablename}({columns}) values ({values}) SELECT SCOPE_IDENTITY()";

            return Convert.ToInt32(_syncDbHelper.ExecuteScalar(query, tx, parametres));
        }

        public void Update(string tablename, IDictionary<string, object> parametres, DbTransaction tx, params Filter[] filters)
        {
            string setQuery = string.Join(",", parametres.Select(x => $"{x.Key} = @{x.Key}"));
            string query = $"update {tablename} set {setQuery} {this.createFilterQuery(filters)}";

            this.fillFiltersToParams(parametres, filters);
            _syncDbHelper.ExecuteNonQuery(query, tx, parametres);
        }

        public IList<T> JoinedGet<T>(string tablename, string[] columns, Join[] joins, Func<IDataReader, T> entityReader, params Filter[] filters)
        {
            var queryColumns = columns.Select(x => $"{tablename}.{x}").ToList();

            var joinColumns = joins.SelectMany(x =>
            {
                if (x.JoinAsTrue)
                {
                    return x.Columns.Select(y => $"{x.JoinAs}.{y} as {x.JoinAs}{y}");
                }
                else
                {
                    return x.Columns.Select(y => $"{x.TableName}.{y}");

                }
            });

            queryColumns.AddRange(joinColumns);

            string allColumns = string.Join(',', queryColumns.ToArray());

            string query = $"select {allColumns} from {tablename}  {this.createJoinQuery(joins)} {this.createFilterQuery(filters)}";

            var parameters = this.createParamsForFilters(filters);

            return _syncDbHelper.GetData(query, entityReader, parameters);
        }

        public IList<T> SimpleGet<T>(string tablename, string[] columns, Func<IDataReader, T> entityReader, params Filter[] filters)
        {
            string columsnQuery = string.Join(",", columns);
            string query = $"select {columsnQuery} from {tablename} {this.createFilterQuery(filters)}";

            var parameters = this.createParamsForFilters(filters);

            return _syncDbHelper.GetData(query, entityReader, parameters);
        }


        private string createFilterQuery(Filter[] filters)
        {
            StringBuilder sb = new StringBuilder();

            bool isFirst = true;

            foreach (var filter in filters ?? new Filter[0])
            {
                if (isFirst)
                {
                    sb.Append("where ");
                }
                else
                {
                    sb.Append(" and  ");
                }

                string column = filter is JoinFilter ? $"{(filter as JoinFilter).TableName}.{filter.Name}" : filter.Name;

                sb.Append($" {column} {this.getComparisonString(filter.Comparison)} @{this.getFilterValueKey(filter)} ");

                isFirst = false;
            }

            return sb.ToString();
        }

        private string getComparisonString(Comparison comparison)
            => comparison switch
            {
                Comparison.Equal => "=",
                Comparison.Greater => ">",
                Comparison.Lower => "<",
                Comparison.GreaterThan => ">=",
                Comparison.LowerThan => "<=",
                _ => throw new NotSupportedException()
            };

        private void fillFiltersToParams(IDictionary<string, object> parameters, Filter[] filters)
        {
            foreach (var filter in filters ?? new Filter[0])
            {
                parameters.Add(this.getFilterValueKey(filter), filter.Value);
            }
        }

        private IDictionary<string, object> createParamsForFilters(Filter[] filters)
        {
            var parameters = new Dictionary<string, object>();

            if (filters == null || filters.Length == 0) return null;

            foreach (var filter in filters)
            {
                parameters.Add(this.getFilterValueKey(filter), filter.Value);
            }

            return parameters;
        }

        private string getFilterValueKey(Filter filter)
        {
            return $"filter{filter.Name}";
        }

        private string createJoinQuery(Join[] joins)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var j in joins)
            {
                if (j.JoinAsTrue)
                {
                    sb.AppendLine($"{j.JoinType} join {j.TableName} as {j.JoinAs} on {j.JoinAs}.{j.JoinColumn} = {j.JoinsToTableName}.{j.JoinsToColumn}");

                }
                else
                {
                    sb.AppendLine($"{j.JoinType} join {j.TableName} on {j.TableName}.{j.JoinColumn} = {j.JoinsToTableName}.{j.JoinsToColumn}");
                }
            }

            return sb.ToString();
        }
    }
}
