// 创建时间：2023-09-04-17:47
// 修改时间：2023-09-05-17:59

#region

#endregion

namespace LiveBoost.Data;

/// <summary>
///     发送消息使登录窗口输入框获取焦点
/// </summary>
public class LoginWndInputBoxFocus : PubSubEvent<bool>
{
    public static readonly List<SubscriptionToken> SubscriptionTokens = new();
}
