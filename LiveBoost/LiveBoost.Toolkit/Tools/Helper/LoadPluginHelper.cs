// 创建时间：2023-09-06-14:33
// 修改时间：2023-09-15-15:41

#region

#endregion

namespace LiveBoost.Toolkit.Tools;

public static class LoadPluginHelper
{
    /// <summary>
    ///     加载插件
    /// </summary>
    /// <param name = "pluginDll" > 插件的DLL文件路径 </param>
    /// <param name = "guid" > GUID </param>
    /// <returns> 启动的进程 </returns>
    public static Process LoadPlugin(string pluginDll, string guid)
    {
        // if (string.IsNullOrWhiteSpace(pluginDll) || string.IsNullOrWhiteSpace(guid))
        // {
        //     throw new ArgumentException("插件的DLL文件路径和GUID不能为空或只包含空格");
        // }
        //
        // if (!File.Exists(pluginDll))
        // {
        //     throw new FileNotFoundException("插件的DLL文件不存在", pluginDll);
        // }

        var startInfo = new ProcessStartInfo
        {
            FileName = pluginDll,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = guid
        };

        var process = new Process {StartInfo = startInfo};
        return process;
    }
}
