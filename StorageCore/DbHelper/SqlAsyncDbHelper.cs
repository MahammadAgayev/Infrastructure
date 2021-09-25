using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Extensions;

namespace StorageCore.DbHelper
{
    public class SqlAsyncDbHelper : IAsyncDbHelper
    {
        private readonly ILogger<SqlAsyncDbHelper> _logger;
        private readonly string _connectionString;

        public SqlAsyncDbHelper(IOptions<SqlDbOptions> options, ILogger<SqlAsyncDbHelper> logger)
            : this(options?.Value, logger)
        { }

        public SqlAsyncDbHelper(SqlDbOptions options, ILogger<SqlAsyncDbHelper> logger)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _connectionString = options.ConnectionString;
        }


        public async Task<int> ExecuteNonQueryAsync(string query, IDictionary<string, object> parameters = null)
          => await this.ExecuteNonQueryAsync(query, this.convertToDbParameterArray(parameters));

        public async Task<int> ExecuteNonQueryAsync(string query, DbTransaction tx, IDictionary<string, object> parameters = null)
         => await this.ExecuteNonQueryAsync(query, tx, this.convertToDbParameterArray(parameters));

        public async Task<int> ExecuteNonQueryAsync(string query, params DbParameter[] parameters)
          => await this.callDbWithResult(this.buildExecuteNonQueryAction(query, tx: null, parameters));

        public async Task<int> ExecuteNonQueryAsync(string query, DbTransaction tx, params DbParameter[] parameters)
         => await this.callDbWithResult(tx, this.buildExecuteNonQueryAction(query, tx, parameters));

        public async Task<object> ExecuteScalarAsync(string query, IDictionary<string, object> parameters = null)
          => await this.ExecuteScalarAsync(query, this.convertToDbParameterArray(parameters));

        public async Task<object> ExecuteScalarAsync(string query, DbTransaction tx, IDictionary<string, object> parameters = null)
          => await this.ExecuteScalarAsync(query, tx, this.convertToDbParameterArray(parameters));

        public async Task<object> ExecuteScalarAsync(string query, params DbParameter[] parameters)
          => await this.callDbWithResult(this.buildExecuteScalarAction(query, tx: null, parameters));

        public async Task<object> ExecuteScalarAsync(string query, DbTransaction tx, params DbParameter[] parameters)
          => await this.callDbWithResult(tx, this.buildExecuteScalarAction(query, tx, parameters));

        public async Task<T> GetScalarAsync<T>(string query, params DbParameter[] parameters)
        {
            object value = await this.ExecuteScalarAsync(query, parameters);

            value = value == DBNull.Value ? default : value;

            return (T)value;
        }

        public async Task<T> GetScalarAsync<T>(string query, IDictionary<string, object> parameters = null)
         => await this.GetScalarAsync<T>(query, this.convertToDbParameterArray(parameters));

        public async Task<IList<T>> GetDataAsync<T>(string query, Func<IDataReader, T> entityReader, IDictionary<string, object> parameters = null)
            => await this.GetDataAsync(query, entityReader, this.convertToDbParameterArray(parameters));

        public async Task<IList<T>> GetDataAsync<T>(string query, Func<IDataReader, T> entityReader, params DbParameter[] parameters)
          => await this.callDbWithResult(this.buildGetDataAction(query, tx: null, entityReader, parameters));

        public async Task<IList<T>> GetDataAsync<T>(string query, DbTransaction tx, Func<IDataReader, T> entityReader, params DbParameter[] parameters)
          => await this.callDbWithResult(tx, this.buildGetDataAction(query, tx, entityReader, parameters));

        public DbParameter CreateParameter(string name, object value, DbType type, ParameterDirection direction = ParameterDirection.Input)
            => new SqlParameter
            {
                ParameterName = name,
                Value = value,
                DbType = type,
                Direction = direction
            };

        public async Task<DbTransaction> GetDbTransaction()
        {
            var connection = await this.getConnectionAsync();

            return await connection.BeginTransactionAsync();
        }


        private Func<DbConnection, Task<int>> buildExecuteNonQueryAction(string query, DbTransaction tx, params DbParameter[] parameters)
        {
            return async conn =>
            {
                using (var command = this.createCommand(conn, query, parameters))
                {
                    var sw = Stopwatch.StartNew();

                    if (tx != null)
                    {
                        command.Transaction = tx;
                    }

                    var result = await command.ExecuteNonQueryAsync().AnyContext();
                    this.logQuery(query, sw.Elapsed.TotalMilliseconds, parameters);

                    return result;
                }
            };
        }

        private Func<DbConnection, Task<object>> buildExecuteScalarAction(string query, DbTransaction tx, params DbParameter[] parameters)
        {
            return async conn =>
            {
                using (var command = this.createCommand(conn, query, parameters))
                {
                    var sw = Stopwatch.StartNew();

                    if (tx != null)
                    {
                        command.Transaction = tx;
                    }

                    var result = await command.ExecuteScalarAsync().AnyContext();
                    this.logQuery(query, sw.Elapsed.TotalMilliseconds, parameters);

                    return result;
                }
            };
        }

        private Func<DbConnection, Task<IList<T>>> buildGetDataAction<T>(string query, DbTransaction tx, Func<IDataReader, T> entityReader, params DbParameter[] parameters)
        {
            return async conn =>
            {
                using (var command = this.createCommand(conn, query, parameters))
                {

                    if (tx != null)
                    {
                        command.Transaction = tx;
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var sw = Stopwatch.StartNew();

                        var result = await this.readList(reader as SqlDataReader, entityReader).AnyContext();

                        this.logQuery(query, sw.Elapsed.TotalMilliseconds, parameters);

                        return result;
                    }
                }
            };
        }

        private async Task<T> callDbWithResult<T>(Func<DbConnection, Task<T>> func)
        {
            using (var conn = await this.getConnectionAsync())
            {
                return await this.executeDbAction(conn, func);
            }
        }

        private async Task<T> callDbWithResult<T>(DbTransaction tx, Func<DbConnection, Task<T>> func)
        {
            var conn = this.getTransactionConnection(tx);

            return await this.executeDbAction(conn, func);
        }

        private async Task<T> executeDbAction<T>(DbConnection conn, Func<DbConnection, Task<T>> func)
        {
            try
            {
                return await func(conn).AnyContext();
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

        private async Task<DbConnection> getConnectionAsync()
        {
            _logger.LogInformation("Connecting to database");

            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

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
                throw new InvalidOperationException("Transaction and/or connection associated with it is null.");

            return tx.Connection;
        }

        private async Task<IList<T>> readList<T>(SqlDataReader reader, Func<IDataReader, T> entityReader)
        {
            var list = new List<T>();

            while (await reader.ReadAsync())
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