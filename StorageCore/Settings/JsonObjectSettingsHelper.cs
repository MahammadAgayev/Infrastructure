using Newtonsoft.Json;
using StorageCore.Settings.Abstraction;

namespace StorageCore.Settings
{
    public class JsonObjectSettingsHelper : IObjectSettingsHelper
    {
        private readonly ISettingsHelper _settingsHelper;

        public JsonObjectSettingsHelper(ISettingsHelper settingsHelper)
        {
            _settingsHelper = settingsHelper;
        }

        public void Save(string key, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            _settingsHelper.Save(key, json);
        }

        public T Get<T>(string key)
        {
            string json = _settingsHelper.Get(key);

            if (json == null)
            {
                return default;
            }

            var obj = JsonConvert.DeserializeObject<T>(json);

            return obj;
        }
    }
}
