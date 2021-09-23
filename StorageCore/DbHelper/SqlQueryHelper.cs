using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using StorageCore.DbHelper.Abstraction;

namespace StorageCore.DbHelper
{
    public class SqlQueryHelper : IQueryHelper
    {
        public string GetInsertQuery(string tablename, IDictionary<string, object> parametres)
        {
            string columns = string.Join(',', parametres.Select(x => x.Key));
            string values = string.Join(',', parametres.Select(x => $"@{x.Key}"));

            string query = $"insert into {tablename}({columns}) values ({values})";

            return query;
        }

        public string GetJoinedGetQuery(string tablename, string[] columns, Join[] joins, IDictionary<string, object> parametres, params Filter[] filters)
        {
            var queryColumns = columns.Select(x => $"{tablename}.{x}").ToList();

            var joinColumns = joins.SelectMany(x => x.Columns.Select(y => $"{x.TableName}.{y}"));

            queryColumns.AddRange(joinColumns);

            string allColumns = string.Join(',', queryColumns.ToArray());

            string query = $"select {allColumns} from {tablename}  {this.createJoinQuery(joins)} {this.createFilterQuery(filters)}";

            return query;
        }

        public string GetSimpleGetQuery(string tablename, string[] columns, IDictionary<string, object> parametres, params Filter[] filters)
        {
            string columsnQuery = string.Join(",", columns);
            string query = $"select {columsnQuery} from {tablename} {this.createFilterQuery(filters)}";

            return query;
        }

        public string GetUpdateQuery(string tablename, IDictionary<string, object> parametres, params Filter[] filters)
        {
            StringBuilder sb = new StringBuilder();

            var setParameters = parametres.Where(a => filters.Any(x => !x.Name.ToLowerInvariant()
            .Contains(a.Key.ToLowerInvariant())));

            string setQuery = string.Join(",", setParameters.Select(x => $"{x.Key} = @{x.Key}"));
            string query = $"update {tablename} set {setQuery} {this.createFilterQuery(filters)}";

            return query;
        }


        public string createJoinQuery(Join[] joins)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var j in joins)
            {
                sb.AppendLine($"{j.JoinType} join {j.TableName} on {j.TableName}.{j.JoinColumn} = {j.JoinsToTableName}.{j.JoinsToColumn}");
            }

            return sb.ToString();
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

                sb.Append($" {column} {this.getComparisonString(filter.Comparison)} @{filter.Name} ");

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
    }
}