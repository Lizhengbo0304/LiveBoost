// 创建时间：2023-09-04-11:27
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Tools;

public static partial class UrlHelper
{
    #region Const

    /// <summary>
    ///     AES
    ///     加密密钥
    /// </summary>
    public const string Aes = "JH2VSIK9CNA76SC3";

    /// <summary>
    ///     MD5加密前缀
    /// </summary>
    public const string Md5Str = "Aq1Nmu8J1itNfcvxAFqeeJXIyUoYq7Ge";

    public const string KfbMd5 = "sdy@123";

    #endregion

    #region Base-Event

    /// <summary>
    ///     发送POST请求并处理响应数据的通用方法。
    /// </summary>
    /// <typeparam name="T"> 返回结果的类型。 </typeparam>
    /// <param name="url"> 要发送请求的URL。 </param>
    /// <param name="para"> 要发送的参数对象。 </param>
    /// <param name="successDataHandler"> 用于处理成功响应数据的委托。 </param>
    /// <param name="fieldDataHandler"> 用于处理字段错误响应数据的委托。 </param>
    /// <param name="exceptionHandler"> 用于处理异常情况的委托。 </param>
    /// <returns> 处理后的结果。 </returns>
    public static async Task<T?> Post<T>(this string url, object para, Func<string, T?> successDataHandler, Func<string, T?> fieldDataHandler, Func<Exception, T?> exceptionHandler)
    {
        try
        {
            if (url.StartsWith("https") || url.StartsWith("http"))
            {
                FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
            }

            // 发送POST请求并异步接收字符串响应
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}").PostJsonAsync(para).ReceiveString().ConfigureAwait(false);

            var jsonToken = JToken.Parse(result);
            switch (jsonToken)
            {
                // 检查是否登录状态已过期(微服务)
                case JObject jobj when jobj["code"]?.Value<int>() == 401:
                    // 执行退出到登录界面的操作
                    MessageBox.Warning(jobj["msg"]?.ToString() ?? "用户登录状态已过期，请重新登录", "用户验证");
                    AppProgram.Instance.LogoutInit(); // 这里需要根据您的实际需求实现退出逻辑和导航到登录界面的操作
                    return default;
                // 检查响应中的"success"字段是否为1，表示请求成功(老媒资成功标记 success = 1)
                case JObject jobj when jobj["success"]?.Value<int>() == 1:
                    // 调用传入的successDataHandler委托以处理JSON数据并返回结果
                    return successDataHandler(result);
                // 检查响应中的"code"字段是否为200，表示请求成功(微服务成功标记 code = 200)
                case JObject jobj when jobj["code"]?.Value<int>() == 200:
                    // 调用传入的successDataHandler委托以处理JSON数据并返回结果
                    return successDataHandler(result);
                case JArray:
                    return successDataHandler(result);
                default:
                    return fieldDataHandler(result);
            }
        }
        catch (Exception e)
        {
            // 使用传入的异常处理方法
            return exceptionHandler(e);
        }
    }

    /// <summary>
    ///     发送Get请求并处理响应数据的通用方法。
    /// </summary>
    /// <typeparam name="T"> 返回结果的类型。 </typeparam>
    /// <param name="url"> 要发送请求的URL。 </param>
    /// <param name="successDataHandler"> 用于处理成功响应数据的委托。 </param>
    /// <param name="fieldDataHandler"> 用于处理字段错误响应数据的委托。 </param>
    /// <param name="exceptionHandler"> 用于处理异常情况的委托。 </param>
    /// <returns> 处理后的结果。 </returns>
    public static async Task<T?> Get<T>(this IFlurlRequest url, Func<string, T?> successDataHandler, Func<string, T?> fieldDataHandler, Func<Exception, T?> exceptionHandler)
    {
        try
        {
            if (url.Url.Root.StartsWith("https") || url.Url.Root.StartsWith("http"))
            {
                FlurlHttp.ConfigureClient(url.Url.Root, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
            }

            // 发送POST请求并异步接收字符串响应
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}").GetStringAsync().ConfigureAwait(false);
            // 解析JSON响应
            var jsonToken = JToken.Parse(result);
            switch (jsonToken)
            {
                // 检查是否登录状态已过期(微服务)
                case JObject jobj when jobj["code"]?.Value<int>() == 401:
                    // 执行退出到登录界面的操作
                    MessageBox.Warning(jobj["msg"]?.ToString() ?? "用户登录状态已过期，请重新登录", "用户验证");
                    AppProgram.Instance.LogoutInit(); // 这里需要根据您的实际需求实现退出逻辑和导航到登录界面的操作
                    return default;
                // 检查响应中的"success"字段是否为1，表示请求成功(老媒资成功标记 success = 1)
                case JObject jobj when jobj["success"]?.Value<int>() == 1:
                    // 调用传入的successDataHandler委托以处理JSON数据并返回结果
                    return successDataHandler(result);
                // 检查响应中的"code"字段是否为200，表示请求成功(微服务成功标记 code = 200)
                case JObject jobj when jobj["code"]?.Value<int>() == 200:
                    // 调用传入的successDataHandler委托以处理JSON数据并返回结果
                    return successDataHandler(result);
                case JArray:
                    return successDataHandler(result);
                default:
                    return fieldDataHandler(result);
            }
        }
        catch (Exception e)
        {
            // 使用传入的异常处理方法
            return exceptionHandler(e);
        }
    }

    /// <summary>
    ///     发送Get请求并处理响应数据的通用方法。
    /// </summary>
    /// <typeparam name="T"> 返回结果的类型。 </typeparam>
    /// <param name="url"> 要发送请求的URL。 </param>
    /// <param name="successDataHandler"> 用于处理成功响应数据的委托。 </param>
    /// <param name="fieldDataHandler"> 用于处理字段错误响应数据的委托。 </param>
    /// <param name="exceptionHandler"> 用于处理异常情况的委托。 </param>
    /// <returns> 处理后的结果。 </returns>
    public static async Task<T?> Get<T>(this string url, Func<string, T?> successDataHandler, Func<string, T?> fieldDataHandler, Func<Exception, T?> exceptionHandler)
    {
        try
        {
            if (url.StartsWith("https") || url.StartsWith("http"))
            {
                FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
            }

            // 发送POST请求并异步接收字符串响应
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}").GetStringAsync().ConfigureAwait(false);

            var jsonToken = JToken.Parse(result);
            switch (jsonToken)
            {
                // 检查是否登录状态已过期(微服务)
                case JObject jobj when jobj["code"]?.Value<int>() == 401:
                    // 执行退出到登录界面的操作
                    MessageBox.Warning(jobj["msg"]?.ToString() ?? "用户登录状态已过期，请重新登录", "用户验证");
                    AppProgram.Instance.LogoutInit(); // 这里需要根据您的实际需求实现退出逻辑和导航到登录界面的操作
                    return default;
                // 检查响应中的"success"字段是否为1，表示请求成功(老媒资成功标记 success = 1)
                case JObject jobj when jobj["success"]?.Value<int>() == 1:
                    // 调用传入的successDataHandler委托以处理JSON数据并返回结果
                    return successDataHandler(result);
                // 检查响应中的"code"字段是否为200，表示请求成功(微服务成功标记 code = 200)
                case JObject jobj when jobj["code"]?.Value<int>() == 200:
                    // 调用传入的successDataHandler委托以处理JSON数据并返回结果
                    return successDataHandler(result);
                case JArray:
                    return successDataHandler(result);
                default:
                    return fieldDataHandler(result);
            }
        }
        catch (Exception e)
        {
            // 使用传入的异常处理方法
            return exceptionHandler(e);
        }
    }

    /// <summary>
    ///     发送Delete请求并处理响应数据的通用方法。
    /// </summary>
    /// <typeparam name="T"> 返回结果的类型。 </typeparam>
    /// <param name="url"> 要发送请求的URL。 </param>
    /// <param name="successDataHandler"> 用于处理成功响应数据的委托。 </param>
    /// <param name="fieldDataHandler"> 用于处理字段错误响应数据的委托。 </param>
    /// <param name="exceptionHandler"> 用于处理异常情况的委托。 </param>
    /// <returns> 处理后的结果。 </returns>
    public static async Task<T?> Delete<T>(this string url, Func<string, T?> successDataHandler, Func<string, T?> fieldDataHandler, Func<Exception, T?> exceptionHandler)
    {
        try
        {
            if (url.StartsWith("https") || url.StartsWith("http"))
            {
                FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
            }

            // 发送POST请求并异步接收字符串响应
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}").DeleteAsync().ReceiveString().ConfigureAwait(false);

            var jsonToken = JToken.Parse(result);
            switch (jsonToken)
            {
                // 检查是否登录状态已过期(微服务)
                case JObject jobj when jobj["code"]?.Value<int>() == 401:
                    // 执行退出到登录界面的操作
                    MessageBox.Warning(jobj["msg"]?.ToString() ?? "用户登录状态已过期，请重新登录", "用户验证");
                    AppProgram.Instance.LogoutInit(); // 这里需要根据您的实际需求实现退出逻辑和导航到登录界面的操作
                    return default;
                // 检查响应中的"success"字段是否为1，表示请求成功(老媒资成功标记 success = 1)
                case JObject jobj when jobj["success"]?.Value<int>() == 1:
                    // 调用传入的successDataHandler委托以处理JSON数据并返回结果
                    return successDataHandler(result);
                // 检查响应中的"code"字段是否为200，表示请求成功(微服务成功标记 code = 200)
                case JObject jobj when jobj["code"]?.Value<int>() == 200:
                    // 调用传入的successDataHandler委托以处理JSON数据并返回结果
                    return successDataHandler(result);
                case JArray:
                    return successDataHandler(result);
                default:
                    return fieldDataHandler(result);
            }
        }
        catch (Exception e)
        {
            // 使用传入的异常处理方法
            return exceptionHandler(e);
        }
    }

    /// <summary>
    ///     发送Put请求并处理响应数据的通用方法。
    /// </summary>
    /// <typeparam name="T"> 返回结果的类型。 </typeparam>
    /// <param name="url"> 要发送请求的URL。 </param>
    /// <param name="para"> 要发送的参数对象。 </param>
    /// <param name="successDataHandler"> 用于处理成功响应数据的委托。 </param>
    /// <param name="fieldDataHandler"> 用于处理字段错误响应数据的委托。 </param>
    /// <param name="exceptionHandler"> 用于处理异常情况的委托。 </param>
    /// <returns> 处理后的结果。 </returns>
    public static async Task<T?> Put<T>(this string url, object para, Func<string, T?> successDataHandler, Func<string, T?> fieldDataHandler, Func<Exception, T?> exceptionHandler)
    {
        try
        {
            if (url.StartsWith("https") || url.StartsWith("http"))
            {
                FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
            }

            // 发送POST请求并异步接收字符串响应
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}").PutJsonAsync(para).ReceiveString().ConfigureAwait(false);

            var jsonToken = JToken.Parse(result);
            switch (jsonToken)
            {
                // 检查是否登录状态已过期(微服务)
                case JObject jobj when jobj["code"]?.Value<int>() == 401:
                    // 执行退出到登录界面的操作
                    MessageBox.Warning(jobj["msg"]?.ToString() ?? "用户登录状态已过期，请重新登录", "用户验证");
                    AppProgram.Instance.LogoutInit(); // 这里需要根据您的实际需求实现退出逻辑和导航到登录界面的操作
                    return default;
                // 检查响应中的"success"字段是否为1，表示请求成功(老媒资成功标记 success = 1)
                case JObject jobj when jobj["success"]?.Value<int>() == 1:
                    // 调用传入的successDataHandler委托以处理JSON数据并返回结果
                    return successDataHandler(result);
                // 检查响应中的"code"字段是否为200，表示请求成功(微服务成功标记 code = 200)
                case JObject jobj when jobj["code"]?.Value<int>() == 200:
                    // 调用传入的successDataHandler委托以处理JSON数据并返回结果
                    return successDataHandler(result);
                case JArray:
                    return successDataHandler(result);
                default:
                    return fieldDataHandler(result);
            }
        }
        catch (Exception e)
        {
            // 使用传入的异常处理方法
            return exceptionHandler(e);
        }
    }

    /// <summary>
    ///     发送POST请求并处理响应数据的通用方法。
    /// </summary>
    /// <param name="url"> 要发送请求的URL。 </param>
    /// <param name="para"> 要发送的参数对象。 </param>
    /// <param name="successDataHandler"> 用于处理成功响应数据的委托。 </param>
    /// <param name="fieldDataHandler"> 用于处理字段错误响应数据的委托。 </param>
    /// <param name="exceptionHandler"> 用于处理异常情况的委托。 </param>
    public static async Task Post(this string url, object para, Action<string> successDataHandler, Action<string> fieldDataHandler, Action<Exception> exceptionHandler)
    {
        try
        {
            if (url.StartsWith("https") || url.StartsWith("http"))
            {
                FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
            }

            // 发送POST请求并异步接收字符串响应
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}")
                .PostJsonAsync(para)
                .ReceiveString()
                .ConfigureAwait(false);

            // 解析JSON响应
            var jobj = JObject.Parse(result);

            // 检查是否登录状态已过期(微服务)
            if (jobj["code"]?.Value<int>() == 401)
            {
                // 执行退出到登录界面的操作
                MessageBox.Warning(jobj["msg"]?.ToString() ?? "用户登录状态已过期，请重新登录", "用户验证");
                AppProgram.Instance.LogoutInit(); // 这里需要根据您的实际需求实现退出逻辑和导航到登录界面的操作
                return;
            }

            // 检查响应中的"success"字段是否为1，表示请求成功(老媒资成功标记 success = 1)
            if (jobj["success"]?.Value<int>() == 1)
            {
                // 调用传入的successDataHandler委托以处理JSON数据
                successDataHandler(result);
            }
            else
            {
                // 检查响应中的"code"字段是否为200，表示请求成功(微服务成功标记 code = 200)
                if (jobj["code"]?.Value<int>() == 200)
                {
                    // 调用传入的successDataHandler委托以处理JSON数据
                    successDataHandler(result);
                }
                else
                {
                    // 调用传入的fieldDataHandler委托以处理字段错误响应数据
                    fieldDataHandler(result);
                }
            }
        }
        catch (Exception e)
        {
            // 使用传入的异常处理方法
            exceptionHandler(e);
        }
    }

    /// <summary>
    ///     发送Put请求并处理响应数据的通用方法。
    /// </summary>
    /// <param name="url"> 要发送请求的URL。 </param>
    /// <param name="para"> 要发送的参数对象。 </param>
    /// <param name="successDataHandler"> 用于处理成功响应数据的委托。 </param>
    /// <param name="fieldDataHandler"> 用于处理字段错误响应数据的委托。 </param>
    /// <param name="exceptionHandler"> 用于处理异常情况的委托。 </param>
    public static async Task Put(this string url, object para, Action<string> successDataHandler, Action<string> fieldDataHandler, Action<Exception> exceptionHandler)
    {
        try
        {
            if (url.StartsWith("https") || url.StartsWith("http"))
            {
                FlurlHttp.ConfigureClient(url, cli => cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
            }

            // 发送POST请求并异步接收字符串响应
            var result = await url.WithHeader("Authorization", $"Bearer {AppProgram.Instance.LoginUser?.Token}")
                .PutJsonAsync(para)
                .ReceiveString()
                .ConfigureAwait(false);

            // 解析JSON响应
            var jobj = JObject.Parse(result);

            // 检查是否登录状态已过期(微服务)
            if (jobj["code"]?.Value<int>() == 401)
            {
                // 执行退出到登录界面的操作
                MessageBox.Warning(jobj["msg"]?.ToString() ?? "用户登录状态已过期，请重新登录", "用户验证");
                AppProgram.Instance.LogoutInit(); // 这里需要根据您的实际需求实现退出逻辑和导航到登录界面的操作
                return;
            }

            // 检查响应中的"success"字段是否为1，表示请求成功(老媒资成功标记 success = 1)
            if (jobj["success"]?.Value<int>() == 1)
            {
                // 调用传入的successDataHandler委托以处理JSON数据
                successDataHandler(result);
            }
            else
            {
                // 检查响应中的"code"字段是否为200，表示请求成功(微服务成功标记 code = 200)
                if (jobj["code"]?.Value<int>() == 200)
                {
                    // 调用传入的successDataHandler委托以处理JSON数据
                    successDataHandler(result);
                }
                else
                {
                    // 调用传入的fieldDataHandler委托以处理字段错误响应数据
                    fieldDataHandler(result);
                }
            }
        }
        catch (Exception e)
        {
            // 使用传入的异常处理方法
            exceptionHandler(e);
        }
    }

    #endregion
}