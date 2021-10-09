using System.Data;
using StorageCore.DbHelper.Abstraction;
using StorageCore.Settings.Abstraction;

namespace StorageCore.Settings
{
    public class DbSettingsHelper : ISettingsHelper
    {
        private readonly ISyncDbHelper _syncDbHelper;

        public DbSettingsHelper(ISyncDbHelper syncDbHelper)
        {
            _syncDbHelper = syncDbHelper;
        }

        public void Save(string key, string value)
        {
            int affectedRow = _syncDbHelper.ExecuteNonQuery("update settings set jsonvalue = @value where name = @key",
                _syncDbHelper.CreateParameter("key", key, DbType.String),
                _syncDbHelper.CreateParameter("value", value, DbType.String));

            if (affectedRow == 0)
            {
                try
                {
                    _syncDbHelper.ExecuteNonQuery("insert into settings values(@key, @value)",
                    _syncDbHelper.CreateParameter("key", key, DbType.String),
                    _syncDbHelper.CreateParameter("value", value, DbType.String));
                }
                catch
                {
                    _syncDbHelper.ExecuteNonQuery("update settings set jsonvalue = @value where name = @key",
                        _syncDbHelper.CreateParameter("key", key, DbType.String),
                        _syncDbHelper.CreateParameter("value", value, DbType.String));

                }
            }
        }

        public string Get(string key)
        {
            return _syncDbHelper.GetScalar<string>("select jsonvalue from settings where name = @key",
                        _syncDbHelper.CreateParameter("key", key, DbType.String));
        }
    }
}
