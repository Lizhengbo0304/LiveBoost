// 创建时间：2023-09-04-11:58
// 修改时间：2023-10-13-16:16

namespace LiveBoost.Views;

public partial class LoginWindow
{
    public LoginWindow()
    {
        // 初始化窗口组件
        InitializeComponent();

        // 在窗口渲染完成后，将焦点设置到用户名输入框
        ContentRendered += (_, _) => UserNameTextBox.Focus();

        // 订阅焦点切换事件，根据事件参数决定焦点位置
        LoginWndInputBoxFocus.SubscriptionTokens.Add(GlobalEvent.Instance.GetEvent<LoginWndInputBoxFocus>().Subscribe(isName =>
        {
            Dispatcher.Invoke(() =>
            {
                if (isName)
                {
                    // 如果是用户名事件，设置焦点到用户名输入框
                    UserNameTextBox.Focus();
                }
                else
                {
                    // 如果是密码事件，设置焦点到密码输入框
                    UserPasswordBox.Focus();
                }
            });
        }));
    }

    private void UserNameTextBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        // 当用户名输入框获得焦点时，全选文本
        UserNameTextBox.SelectAll();
    }

    private void UserPasswordBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        // 当密码输入框获得焦点时，清空文本
        UserPasswordBox.Clear();
    }
}