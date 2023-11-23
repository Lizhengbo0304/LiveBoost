// 创建时间：2023-09-06-14:33
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Tools;

public static class LoadPluginHelper
{
    /// <summary>
    ///     加载插件
    /// </summary>
    /// <param name="pluginDll"> 插件的DLL文件路径 </param>
    /// <param name="guid"> GUID </param>
    /// <returns> 启动的进程 </returns>
    public static Process LoadPlugin(string pluginDll, string guid)
    {
        if (string.IsNullOrWhiteSpace(pluginDll) || string.IsNullOrWhiteSpace(guid))
        {
            throw new ArgumentException("插件的DLL文件路径和GUID不能为空或只包含空格");
        }

        if (!File.Exists(pluginDll))
        {
            throw new FileNotFoundException("插件的DLL文件不存在", pluginDll);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = pluginDll,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = guid
        };

        var process = new Process { StartInfo = startInfo };
        return process;
    }

    /// <summary>
    ///     加载插件
    /// </summary>
    /// <param name="pluginDll"> 插件的DLL文件路径 </param>
    /// <param name="arguments"> 要传递的参数数组 </param>
    /// <returns> 启动的进程 </returns>
    public static Process LoadPlugin(string pluginDll, params string[]? arguments)
    {
        if (string.IsNullOrEmpty(pluginDll) || string.IsNullOrWhiteSpace(pluginDll))
        {
            throw new ArgumentException("插件的DLL文件路径不能为空或只包含空格");
        }

        if (!File.Exists(pluginDll))
        {
            throw new FileNotFoundException("插件的DLL文件不存在", pluginDll);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = pluginDll,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (arguments is { Length: > 0 })
            // 如果有参数，将它们作为一个字符串连接到命令行
        {
            startInfo.Arguments = string.Join(" ", arguments);
        }

        var process = new Process { StartInfo = startInfo };
        return process;
    }
}