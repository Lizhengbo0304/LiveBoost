// 创建时间：2023-09-04-11:29
// 修改时间：2023-09-06-9:03

namespace LiveBoost.Toolkit.Tools;

public static class ConfigHelper
{
    /// <summary>
    ///     通过Key
    ///     获取对应config的value
    /// </summary>
    /// <param
    ///     name = "configPath" >
    ///     配置文件地址
    /// </param>
    /// <param
    ///     name = "key" >
    ///     key值
    /// </param>
    /// <returns>
    ///     value
    /// </returns>
    public static string? GetValueWithKey(this string configPath, string key)
    {
        try
        {
            var config = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configPath
                },
                ConfigurationUserLevel.None);
            if ( !config.HasFile )
            {
                return string.Empty;
            }
            var value = config.AppSettings.Settings[key]?.Value;
            return value;

        }
        catch ( Exception ex )
        {
            ex.LogFileError($@"读取配置文件失败：\r\nPath={configPath}\r\nKey={key}");
            return string.Empty;
        }
    }

    /// <summary>
    ///     配置节增加一对键值对
    /// </summary>
    /// <param
    ///     name = "configPath" >
    ///     配置文件地址
    /// </param>
    /// <param
    ///     name = "newKey" >
    ///     key
    /// </param>
    /// <param
    ///     name = "newValue" >
    ///     value
    /// </param>
    public static void SetVaule(this string configPath, string newKey, string? newValue)
    {
        try
        {
            var controlConfig = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configPath
                },
                ConfigurationUserLevel.None);
            if ( !controlConfig.HasFile )
            {
                return;
            }
            if ( controlConfig.AppSettings.Settings.AllKeys.ToList().Contains(newKey) )
            {
                controlConfig.AppSettings.Settings.Remove(newKey);
            }
            controlConfig.AppSettings?.Settings?.Add(newKey, newValue);
            controlConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        catch ( Exception e )
        {
            e.LogFileError($"设置Key：{newKey} value：{newValue}失败");
        }
    }
}
