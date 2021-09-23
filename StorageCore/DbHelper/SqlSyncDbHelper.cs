using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using StorageCore.DbHelper.Abstraction;

namespace StorageCore.DbHelper
{
    public class SqlSyncDbHelper : ISyncDbHelper
    {
        private readonly ILogger<SqlAsyncDbHelper> _logger;
        private readonly string _connectionString;

        public SqlSyncDbHelper(SqlDbOptions options, ILogger<SqlAsyncDbHelper> logger)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _connectionString = options.ConnectionString;
        }


        public int ExecuteNonQuery(string query, IDictionary<string, object> parameters = null)
          => this.ExecuteNonQuery(query, this.convertToDbParameterArray(parameters));

        public int ExecuteNonQuery(string query, DbTransaction tx, IDictionary<string, object> parameters = null)
         =>  this.ExecuteNonQuery(query, tx, this.convertToDbParameterArray(parameters));

        public int ExecuteNonQuery(string query, params DbParameter[] parameters)
          => this.callDbWithResult(this.buildExecuteNonQueryAction(query, tx: null, parameters));

        public  int ExecuteNonQuery(string query, DbTransaction tx, params DbParameter[] parameters)
         =>  this.callDbWithResult(tx, this.buildExecuteNonQueryAction(query, tx, parameters));

        public object ExecuteScalar(string query, IDictionary<string, object> parameters = null)
          => this.ExecuteScalar(query, this.convertToDbParameterArray(parameters));

        public object ExecuteScalar(string query, DbTransaction tx, IDictionary<string, object> parameters = null)
          => this.ExecuteScalar(query, tx, this.convertToDbParameterArray(parameters));

        public object ExecuteScalar(string query, params DbParameter[] parameters)
          =>  this.callDbWithResult(this.buildExecuteScalarAction(query, tx: null, parameters));

        public object ExecuteScalar(string query, DbTransaction tx, params DbParameter[] parameters)
          =>  this.callDbWithResult(tx, this.buildExecuteScalarAction(query, tx, parameters));

        public T GetScalar<T>(string query, params DbParameter[] parameters)
        {
            object value =  this.ExecuteScalar(query, parameters);

            value = value == DBNull.Value ? default : value;

            return (T)value;
        }

        public T GetScalar<T>(string query, IDictionary<string, object> parameters = null)
         =>  this.GetScalar<T>(query, this.convertToDbParameterArray(parameters));

        public IList<T> GetData<T>(string query, Func<IDataReader, T> entityReader, IDictionary<string, object> parameters = null)
            =>  this.GetData(query, entityReader, this.convertToDbParameterArray(parameters));

        public IList<T>  GetData<T>(string query, Func<IDataReader, T> entityReader, params DbParameter[] parameters)
          =>  this.callDbWithResult(
               conn =>
              {
                  using (var command = this.createCommand(conn, query, parameters))
                  {
                      using (var reader =  command.ExecuteReader())
                      {
                          var sw = Stopwatch.StartNew();

                          var result = this.readList(reader as SqlDataReader, entityReader);

                          this.logQuery(query, sw.Elapsed.TotalMilliseconds, parameters);

                          return result;
                      }
                  }
              });

        public DbParameter CreateParameter(string name, object value, DbType type, ParameterDirection direction = ParameterDirection.Input)
            => new SqlParameter
            {
                ParameterName = name,
                Value = value,
                DbType = type,
                Direction = direction
            };

        public DbTransaction GetDbTransaction()
        {
            var connection = this.getConnection();

            return connection.BeginTransaction();
        }


        private Func<DbConnection, int> buildExecuteNonQueryAction(string query, DbTransaction tx, params DbParameter[] parameters)
        {
            return conn =>
           {
               using (var command = this.createCommand(conn, query, parameters))
               {
                   var sw = Stopwatch.StartNew();

                   if (tx != null)
                   {
                       command.Transaction = tx;
                   }

                   var result = command.ExecuteNonQuery();
                   this.logQuery(query, sw.Elapsed.TotalMilliseconds, parameters);

                   return result;
               }
           };
        }

        private Func<DbConnection, object> buildExecuteScalarAction(string query, DbTransaction tx, params DbParameter[] parameters)
        {
            return conn =>
            {
                using (var command = this.createCommand(conn, query, parameters))
                {
                    var sw = Stopwatch.StartNew();

                    if (tx != null)
                    {
                        command.Transaction = tx;
                    }

                    var result = command.ExecuteScalar();
                    this.logQuery(query, sw.Elapsed.TotalMilliseconds, parameters);

                    return result;
                }
            };
        }

        private T callDbWithResult<T>(Func<DbConnection, T> func)
        {
            using (var conn = this.getConnection())
            {
                return this.executeDbAction(conn, func);
            }
        }

        private T callDbWithResult<T>(DbTransaction tx, Func<DbConnection, T> func)
        {
            var conn = this.getTransactionConnection(tx);

            return this.executeDbAction(conn, func);
        }

        private T executeDbAction<T>(DbConnection conn, Func<DbConnection, T> func)
        {
            try
            {
                return func(conn);
            }
            catch (SqlException ex)
            {
                _logger.LogCritical(ex, "A database error occurred.");
                throw;
            }
        }

        private DbParameter[] convertToDbParameterArray(IDictionary<string, object> parameters)
            => parameters == null || parameters.Count == 0
                ? Array.Empty<DbParameter>()
                : parameters.Select(parameter => new SqlParameter
                {
                    ParameterName = "@" + parameter.Key,
                    Value = parameter.Value ?? DBNull.Value
                }).ToArray();

        private DbCommand createCommand(DbConnection connection, string sql, params DbParameter[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        private DbConnection getConnection()
        {
            _logger.LogInformation("Connecting to database");

            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(_connectionString);
                connection.Open();

                _logger.LogInformation("Connected to database");

                return connection;
            }
            catch (Exception ex)
            {
                connection?.Dispose();

                throw new InvalidOperationException("Error occurred while obtaining database connection", ex);
            }
        }

        private DbConnection getTransactionConnection(DbTransaction tx)
        {
            if (tx?.Connection == null)
                throw new InvalidOperationException("Transaction and/or connection associated with it is null. please create transaction using 'GetDbTransaction' method.");

            return tx.Connection;
        }

        private IList<T> readList<T>(SqlDataReader reader, Func<IDataReader, T> entityReader)
        {
            var list = new List<T>();

            while (reader.Read())
            {
                list.Add(entityReader(reader));
            }

            _logger.LogInformation($"Number of entries returned: [{list.Count}]");

            return list;
        }

        private void logQuery(string query, double totalMilliseconds, params DbParameter[] parameters)
        {
            _logger.LogInformation(parameters.Length > 0
                ? $"Executed query [{query}] with parameters [{string.Join(";", parameters.Select(p => p.ParameterName + "=" + p.Value))}] in {totalMilliseconds} ms."
                : $"Executed query [{query}] in {totalMilliseconds} ms.");
        }
    }
}