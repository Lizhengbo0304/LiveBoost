// 创建时间：2023-09-04-17:47
// 修改时间：2023-09-06-9:03

#region

#endregion

namespace LiveBoost.Toolkit.Data;

/// <summary>
///     发送消息使登录窗口输入框获取焦点
/// </summary>
public class LoginWndInputBoxFocus : PubSubEvent<bool>
{
    public static readonly List<SubscriptionToken> SubscriptionTokens = new();
}

/// <summary>
///     发送消息使主窗口处理播单变动事件
/// </summary>
public class PushAccessRecordFilesChanged : PubSubEvent<PushAccess>
{
    public static readonly List<SubscriptionToken> SubscriptionTokens = new();
}

/// <summary>
///     关闭播放器子进程
/// </summary>
public class CloseChildPlayerProcess : PubSubEvent
{
    public static readonly List<SubscriptionToken> SubscriptionTokens = new();
}
