namespace StorageCore.Settings.Abstraction
{
    public interface ISettingsHelper
    {
        void Save(string key, string value);

        string Get(string key);
    }
}
