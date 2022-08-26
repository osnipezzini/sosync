using Newtonsoft.Json;
using SOCore.Utils;
using System.Runtime.InteropServices;
using static System.Environment;

namespace SOSync.Common;
public class AppSettings
{
    private static SOSyncConfig? sosyncConfig;
    private static SOSyncConfig SOSyncConfig => sosyncConfig ??= new SOSyncConfig();

    static AppSettings()
    {
        if (sosyncConfig is null)
            LoadSOSyncConfig();
    }

    public static IEnumerable<DatabaseConfig> DatabaseConfig => SOSyncConfig.Databases;

    public static string ConfigPath
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(GetFolderPath(SpecialFolder.CommonApplicationData), "SOTech", AppConstants.AppName);
            }
            else
            {
                return Path.Combine(GetFolderPath(SpecialFolder.Personal), ".sotech", AppConstants.AppName.ToLower());
            }
        }
    }

    public static void Save()
    {
        var filePath = Path.Combine(ConfigPath, "settings.db");
        if (!Directory.Exists(ConfigPath))
        {
            Directory.CreateDirectory(ConfigPath);
        }

        var jsonFile = JsonConvert.SerializeObject(sosyncConfig);
        File.WriteAllText(filePath, SOHelper.Encrypt(jsonFile, AppConstants.Secret));
    }
    public static void LoadSOSyncConfig()
    {
        var filePath = Path.Combine(ConfigPath, "settings.db");

        if (File.Exists(filePath))
        {
            var jsonFile = File.ReadAllText(filePath);
            try
            {
                jsonFile = SOHelper.Decrypt(jsonFile, AppConstants.Secret);
                sosyncConfig = JsonConvert.DeserializeObject<SOSyncConfig>(jsonFile);
            }
            catch (Exception)
            {
                sosyncConfig = new SOSyncConfig();
                return;
            }
            return;
        }
        else
        {
            sosyncConfig = new SOSyncConfig();
            return;
        }
    }
}
