// 创建时间：2023-09-04-11:30
// 修改时间：2023-10-11-11:00

namespace LiveBoost.Toolkit.Tools;

public static class LogHelper
{
    /// <summary>
    ///     错误Log
    /// </summary>
    private static readonly ILog UrlError = LogManager.GetLogger("UrlError");
    private static readonly ILog FileError = LogManager.GetLogger("FileError");
    private static readonly ILog LogErrors = LogManager.GetLogger("LogError");

    /// <summary>
    ///     信息Log
    /// </summary>
    private static readonly ILog UrlInfo = LogManager.GetLogger("UrlInfo");
    private static readonly ILog FileInfo = LogManager.GetLogger("FileInfo");
    private static readonly ILog LogInfos = LogManager.GetLogger("LogInfo");

    static LogHelper()
    {
        var path = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("LiveBoost.Configs.log4net.config");

        XmlConfigurator.Configure(path);
    }

    /// <summary>
    ///     写日志
    /// </summary>
    /// <param
    ///     name = "info" >
    ///     日志信息
    /// </param>
    public static void LogUrlInfo(this string info)
    {
        var stackTrace = new StackTrace();

        var callingMethods = stackTrace.GetFrames();

        if ( callingMethods is not {Length: > 0} )
        {
            UrlInfo.Info(info);
            return;
        }

        var callingMethodNames = GetCallingMethodNames(callingMethods);
        UrlInfo.Info($"{callingMethodNames}\n{info}");
    }

    /// <summary>
    ///     写日志
    /// </summary>
    /// <param
    ///     name = "info" >
    ///     日志信息
    /// </param>
    public static void LogFileInfo(this string info)
    {
        var stackTrace = new StackTrace();

        var callingMethods = stackTrace.GetFrames();

        if ( callingMethods is not {Length: > 0} )
        {
            FileInfo.Info(info);
            return;
        }

        var callingMethodNames = GetCallingMethodNames(callingMethods);
        FileInfo.Info($"{callingMethodNames}\n{info}");
    }

    /// <summary>
    ///     写日志
    /// </summary>
    /// <param
    ///     name = "info" >
    ///     日志信息
    /// </param>
    public static void LogInfo(this string info)
    {
        var stackTrace = new StackTrace();
        var callingMethods = stackTrace.GetFrames();

        if ( callingMethods is not {Length: > 0} )
        {
            LogInfos.Info(info);
            return;
        }

        var callingMethodNames = GetCallingMethodNames(callingMethods);
        LogInfos.Info($"{callingMethodNames}\n{info}");
    }

    /// <summary>
    ///     写日志
    /// </summary>
    /// <param
    ///     name = "se" >
    ///     异常信息
    /// </param>
    /// <param
    ///     name = "info" >
    ///     日志信息
    /// </param>
    public static void LogError(this Exception se, string info)
    {
        var stackTrace = new StackTrace();
        var callingMethods = stackTrace.GetFrames();

        if ( callingMethods is not {Length: > 0} )
        {
            LogErrors.Error(info);
            return;
        }

        var callingMethodNames = GetCallingMethodNames(callingMethods);
        LogErrors.Error($"{callingMethodNames}\n{info}");
    }

    /// <summary>
    ///     写日志
    /// </summary>
    /// <param
    ///     name = "se" >
    ///     异常信息
    /// </param>
    /// <param
    ///     name = "info" >
    ///     日志信息
    /// </param>
    public static void LogUrlError(this Exception se, string info)
    {
        var stackTrace = new StackTrace();
        var callingMethods = stackTrace.GetFrames();

        if ( callingMethods is not {Length: > 0} )
        {
            UrlError.Error(info);
            return;
        }

        var callingMethodNames = GetCallingMethodNames(callingMethods);
        UrlError.Error($"{callingMethodNames}\n{info}");
    }

    /// <summary>
    ///     写日志
    /// </summary>
    /// <param
    ///     name = "se" >
    ///     异常信息
    /// </param>
    /// <param
    ///     name = "info" >
    ///     日志信息
    /// </param>
    public static void LogFileError(this Exception se, string info)
    {
        var stackTrace = new StackTrace();
        var callingMethods = stackTrace.GetFrames();

        if ( callingMethods is not {Length: > 0} )
        {
            FileError.Error(info);
            return;
        }

        var callingMethodNames = GetCallingMethodNames(callingMethods);
        FileError.Error($"{callingMethodNames}\n{info}");
    }

    // 获取调用方法的名称
    private static string GetCallingMethodNames(StackFrame[] callingMethods)
    {
        var callingMethodNames = string.Empty;

        for ( var i = callingMethods.Length - 1; i >= 0; i-- )
        {
            var method = callingMethods[i].GetMethod();

            if ( method?.DeclaringType != null && !IsSystemOrFrameworkType(method.DeclaringType) )
            {
                callingMethodNames += $"{method.DeclaringType}.{method.Name}\n";
            }
        }

        return callingMethodNames;
    }

    // 判断方法是否属于系统或框架类型
    private static bool IsSystemOrFrameworkType(Type type)
    {
        var typeName = type.ToString();
        return typeName.StartsWith("System") || typeName.StartsWith("MS");
    }
}
