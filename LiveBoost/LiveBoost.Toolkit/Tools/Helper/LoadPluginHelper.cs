// 创建时间：2023-06-06-14:22
// 修改时间：2023-07-18-9:37

#region

using System.Diagnostics;

#endregion

namespace LiveBoost.Toolkit.Tools;

public static class LoadPluginHelper
{
    public static Process LoadPlugin(string pluginDll, string guid)
    {
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
