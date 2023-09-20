// 创建时间：2023-09-04-11:29
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Toolkit.Tools;

public static class ConfigHelper
{
    /// <summary>
    ///     通过Key获取对应config的value
    /// </summary>
    /// <param name = "configPath" > 配置文件地址 </param>
    /// <param name = "key" > key值 </param>
    /// <returns> value </returns>
    public static string? GetValueWithKey(this string configPath, string key)
    {
        try
        {
            // 打开配置文件
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

            // 获取对应键的值
            var value = config.AppSettings.Settings[key]?.Value;
            return value;
        }
        catch ( Exception ex )
        {
            // 记录错误信息并抛出异常
            ex.LogFileError($@"读取配置文件失败：\r\nPath={configPath}\r\nKey={key}");
            return string.Empty;
        }
    }

    /// <summary>
    ///     配置节增加一对键值对
    /// </summary>
    /// <param name = "configPath" > 配置文件地址 </param>
    /// <param name = "newKey" > key </param>
    /// <param name = "newValue" > value </param>
    public static void SetValue(this string configPath, string newKey, string? newValue)
    {
        try
        {
            // 打开配置文件
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

            // 检查是否存在相同的键，如果存在则先移除
            if ( controlConfig.AppSettings.Settings.AllKeys.ToList().Contains(newKey) )
            {
                controlConfig.AppSettings.Settings.Remove(newKey);
            }

            // 添加或修改键值对
            controlConfig.AppSettings?.Settings?.Add(newKey, newValue);

            // 一次性保存和刷新配置文件
            controlConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        catch ( Exception e )
        {
            // 记录错误信息并抛出异常
            e.LogFileError($"设置Key：{newKey} value：{newValue}失败");
        }
    }
}
