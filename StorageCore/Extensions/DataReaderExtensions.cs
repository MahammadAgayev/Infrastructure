using System;
using System.Data;

namespace StorageCore.Extensions
{
    public static class DataReaderExtensions
    {
        public static T Get<T>(this IDataReader reader, string name)
        {
            object value = reader[name];
            T result = default;

            if (!(value is DBNull) && value != null)
            {
                result = (T)value;
            }

            return result;
        }
    }
}