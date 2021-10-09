using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageCore.Settings.Abstraction
{
    public interface IObjectSettingsHelper
    {
        void Save(string key, object value);

        T Get<T>(string key);
    }
}
